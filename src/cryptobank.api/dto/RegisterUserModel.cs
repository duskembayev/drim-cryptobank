namespace cryptobank.api.dto;

public record RegisterUserModel(string Email, string Password, DateOnly DateOfBirth);