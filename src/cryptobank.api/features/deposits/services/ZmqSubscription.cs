using System.Buffers.Binary;
using cryptobank.api.features.deposits.notifications;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetMQ;
using NetMQ.Sockets;

namespace cryptobank.api.features.deposits.services;

[Singleton<IHostedService>]
internal sealed class ZmqSubscription : IHostedService, IHealthCheck
{
    private const string ConnectionStringName = "bitcoin-zmq";

    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;
    private readonly ILogger<ZmqSubscription> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task? _runTask;

    public ZmqSubscription(IConfiguration configuration, IMediator mediator, ILogger<ZmqSubscription> logger)
    {
        _configuration = configuration;
        _mediator = mediator;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        var healthCheckResult = _runTask is {IsCompleted: false}
            ? HealthCheckResult.Healthy()
            : new HealthCheckResult(context.Registration.FailureStatus);

        return Task.FromResult(healthCheckResult);
    }

    public Task StartAsync(CancellationToken _)
    {
        _runTask = Task.Run(() => RunAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        return _runTask?.WaitAsync(cancellationToken) ?? Task.CompletedTask;
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        const string rawtxTopic = "rawtx";
        const string rawblockTopic = "rawblock";
        const string sequenceTopic = "sequence";

        using var socket = new SubscriberSocket(_configuration.GetConnectionString(ConnectionStringName));

        socket.Subscribe(rawtxTopic);
        socket.Subscribe(rawblockTopic);
        socket.Subscribe(sequenceTopic);

        var seqByTopics = new Dictionary<string, uint>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await socket.ReceiveMultipartMessageAsync(cancellationToken: cancellationToken);
            if (message.FrameCount == 3)
                throw new FormatException("Invalid message format");

            var topic = message.Pop().ConvertToString();
            var body = message.Pop().Buffer;
            var seq = BinaryPrimitives.ReadUInt32LittleEndian(message.Pop().Buffer);

            if (seqByTopics.TryGetValue(topic, out var lastSeq) && ++lastSeq != seq)
                _logger.LogWarning("Missed {MissedCount} messages for topic {Topic}", seq - lastSeq, topic);

            seqByTopics[topic] = seq;
            INotification ntf = topic switch
            {
                rawblockTopic => new ZmqBlockNotification(body),
                rawtxTopic => new ZmqTransactionNotification(body),
                sequenceTopic => new ZmqSequenceNotification(body),
                _ => throw new Exception("Unknown topic")
            };

            await _mediator.Publish(ntf, cancellationToken);
        }
    }
}