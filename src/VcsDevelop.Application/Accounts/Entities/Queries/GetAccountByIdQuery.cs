namespace VcsDevelop.Application.Accounts.Entities.Queries;

public sealed class GetAccountByIdQuery
{
    public Guid Id { get; }

    private GetAccountByIdQuery(Guid id)
    {
        Id = id;
    }

    public static GetAccountByIdQuery Create(Guid id)
    {
        return new GetAccountByIdQuery(id);
    }
}
