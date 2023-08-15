using cryptobank.api;

await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
    .Build()
    .RunAsync();
