namespace PFP.Application.Common.Exceptions
{
    public sealed class AlreadySubmittedException(string message) : Exception(message)
    {
    }
}
