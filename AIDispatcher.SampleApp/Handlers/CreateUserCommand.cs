using AIDispatcher.Dispatcher;
using FluentValidation;

namespace AIDispatcher.SampleApp.Handlers;

public class CreateUserCommand
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;

    public CreateUserCommand() { }
    public CreateUserCommand(string name, string email)
    {
        Name = name;
        Email = email;
    }
}

public class CreateUserHandler : IDispatcherHandler<CreateUserCommand, string>
{
    public Task<string> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"User {request.Name} dengan email {request.Email} berhasil dibuat.");
    }
}

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}