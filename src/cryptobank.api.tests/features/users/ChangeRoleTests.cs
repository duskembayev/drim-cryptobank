﻿using System.Net;
using cryptobank.api.db;
using cryptobank.api.errors;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.requests;
using cryptobank.api.tests.extensions;
using cryptobank.api.tests.fixtures;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace cryptobank.api.tests.features.users;

public class ChangeRoleTests : IClassFixture<ApplicationFixture>
{
    private readonly HttpClient _client;
    private readonly ApplicationFixture _fixture;

    public ChangeRoleTests(ApplicationFixture fixture)
    {
        _fixture = fixture;

        _fixture.Authorize(_fixture.Administrator);
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task ShouldChangeRole()
    {
        var userId = _fixture.User.Id;

        var res = await _client.POSTAsync<ChangeRoleRequest, ProblemDetails>("/user/changeRole",
            new ChangeRoleRequest
            {
                UserId = userId,
                Roles = new[] {Role.Analyst, Role.User}
            });

        res.ShouldBeOk();
        res.Result.ShouldBeNull();

        using var scope = _fixture.AppFactory.Services.CreateScope();

        var user = await scope.ServiceProvider
            .GetRequiredService<CryptoBankDbContext>()
            .Users
            .Include(u => u.Roles)
            .SingleAsync(u => u.Id == userId);

        user.Roles.ShouldContain(r => r.Name == Role.User);
        user.Roles.ShouldContain(r => r.Name == Role.Analyst);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenInvalidUserId()
    {
        var res = await _client.POSTAsync<ChangeRoleRequest, ProblemDetails>("/user/changeRole",
            new ChangeRoleRequest
            {
                UserId = 11,
                Roles = new[] {Role.Analyst, Role.User}
            });

        res.ShouldBeValidationProblem(
            HttpStatusCode.BadRequest,
            "userId",
            GeneralErrorCodes.InvalidUser);
    }
}