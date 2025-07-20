using FluentValidation.Results;

namespace AIDispatcher.Dispatcher;

/// <summary>
/// Represents a standardized validation exception used by the dispatcher pipeline.
/// It wraps FluentValidation errors and exposes simplified error messages for consumers.
/// </summary>
public class DispatcherValidationException : Exception
{
    /// <summary>
    /// The list of validation errors.
    /// </summary>
    public IReadOnlyList<ValidationFailure> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherValidationException"/> class.
    /// </summary>
    /// <param name="failures">The list of validation failures.</param>
    public DispatcherValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.")
    {
        Errors = failures?.ToList() ?? new List<ValidationFailure>();
    }

    /// <summary>
    /// Returns a flat list of validation error messages.
    /// </summary>
    public override string ToString()
    {
        var messages = Errors.Select(e => $"- {e.PropertyName}: {e.ErrorMessage}");
        return $"{Message}{Environment.NewLine}{string.Join(Environment.NewLine, messages)}";
    }
}