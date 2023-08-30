using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.users.requests;

public class ChangeRoleRequest : IRequest
{
    public int UserId { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();

    public class Validator : AbstractValidator<ChangeRoleRequest>
    {
        public Validator()
        {
            RuleFor(request => request.UserId).ValidUserId();

            RuleFor(request => request.Roles)
                .NotEmpty()
                .WithErrorCode("users:change-role:roles_empty")
                .WithMessage("At least one role must be specified");

            RuleFor(request => request.Roles)
                .Must(roles => roles.All(Role.Detached.Exists))
                .WithErrorCode("users:change-role:roles_invalid")
                .WithMessage("One or more roles are invalid");
        }
    }
}