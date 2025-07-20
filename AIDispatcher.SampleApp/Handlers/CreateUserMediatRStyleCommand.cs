using AIDispatcher.Dispatcher;
using FluentValidation;

namespace AIDispatcher.SampleApp.Handlers;
public class CreateUserMediatrStyleCommand : Contact, IRequest<string>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;

    public CreateUserMediatrStyleCommand() { }
    public CreateUserMediatrStyleCommand(string name, string email)
    {
        Name = name;
        Email = email;
    }
}

public class CreateUserMediatrStyleHandler : IRequestHandler<CreateUserMediatrStyleCommand, string>
{
    public Task<string> HandleAsync(CreateUserMediatrStyleCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"User {request.Name} dengan email {request.Email} berhasil dibuat.");
    }
}

public class CreateUserMediatrStyleValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserMediatrStyleValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}