﻿using cryptobank.api.features.users.domain;
using MediatR;

namespace cryptobank.api.features.users;

public class RegisterUserRequest : IRequest<User>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;
}