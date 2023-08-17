﻿using cryptobank.api.config;
using cryptobank.api.dal.users;
using cryptobank.api.dto;
using cryptobank.api.utils.environment;
using cryptobank.api.utils.security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace cryptobank.api.controllers;

[Route("user")]
public class UserController : ControllerBase
{
    private readonly IEmailFormatValidator _emailFormatValidator;
    private readonly IPasswordHashAlgorithm _passwordHashAlgorithm;
    private readonly IPasswordStrengthValidator _passwordStrengthValidator;
    private readonly IUserRepository _userRepository;
    private readonly ITimeProvider _timeProvider;
    private readonly IOptions<RegisterUserOptions> _options;

    public UserController(
        IEmailFormatValidator emailFormatValidator,
        IPasswordHashAlgorithm passwordHashAlgorithm,
        IPasswordStrengthValidator passwordStrengthValidator,
        IUserRepository userRepository,
        ITimeProvider timeProvider,
        IOptions<RegisterUserOptions> options)
    {
        _emailFormatValidator = emailFormatValidator;
        _passwordHashAlgorithm = passwordHashAlgorithm;
        _passwordStrengthValidator = passwordStrengthValidator;
        _userRepository = userRepository;
        _timeProvider = timeProvider;
        _options = options;
    }

    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterUserModel model, CancellationToken cancellationToken)
    {
        if (!_emailFormatValidator.Validate(model.Email))
            return BadRequest("Email is not valid");
        
        if (!_passwordStrengthValidator.Validate(model.Password))
            return BadRequest("Password is not strong enough");
        
        if (model.DateOfBirth > _timeProvider.Today.AddYears(-_options.Value.MinimumAge))
            return BadRequest("User must be at least 18 years old");
        
        var user = await _userRepository.GetByEmailAsync(model.Email, cancellationToken);
        
        if (user is not null)
            return Conflict("User already exists");

        var role = await GetRoleAsync(model, cancellationToken);
        var passwordSalt = _passwordHashAlgorithm.GenerateSalt(_options.Value.PasswordSaltSize);
        var passwordHash = _passwordHashAlgorithm.ComputeHash(model.Password, passwordSalt, _options.Value.PasswordHashSize);

        user = new User
        {
            Email = model.Email,
            Role = role,
            PasswordSalt = passwordSalt,
            PasswordHash = passwordHash,
            DateOfBirth = model.DateOfBirth,
            DateOfRegistration = _timeProvider.UtcNow,
        };
        
        await _userRepository.AddAsync(user, cancellationToken);
        return Ok();
    }

    private async Task<RoleId> GetRoleAsync(RegisterUserModel model, CancellationToken cancellationToken)
    {
        if (model.Email.Equals(_options.Value.FallbackAdminEmail, StringComparison.OrdinalIgnoreCase)
            && await _userRepository.GetCountAsync(RoleId.Administrator, cancellationToken) == 0)
            return RoleId.Administrator;

        return RoleId.User;
    }
}