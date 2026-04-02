namespace GitDevelop.Core.Application;

public interface IRequestContext
{
    Guid? AccountId { get; }
    Guid GetRequiredAccountId();
}
