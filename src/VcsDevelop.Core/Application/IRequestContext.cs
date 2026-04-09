namespace VcsDevelop.Core.Application;

public interface IRequestContext
{
    Guid? AccountId { get; }
    Guid GetRequiredAccountId();
}
