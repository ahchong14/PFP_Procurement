using FluentValidation.Results;

namespace PFP.Application.Common.Exceptions
{
    public sealed class ValidationException : Exception
    {
        public ValidationException() : base("One or more validation failures occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}
