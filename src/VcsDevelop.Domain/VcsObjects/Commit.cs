using System.Security.Cryptography;
using System.Text;

namespace VcsDevelop.Domain.VcsObjects;

public sealed class Commit
{
    public string Id { get; private init; }
    public Guid DocumentId { get; private init; }
    public string RootTreeId { get; private init; }
    public IReadOnlyCollection<CommitParent> ParentIds { get; private init; }
    public Guid AccountId { get; private init; }
    public CommitMessage Message { get; private init; }
    public DateTime CreatedAt { get; private init; }

    // EF only
    private Commit()
    {
        Id = null!;
        RootTreeId = null!;
        ParentIds = new HashSet<CommitParent>();
        Message = null!;
    }

    public Commit(
        string id,
        Guid documentId,
        string rootTreeId,
        IReadOnlyCollection<string> parentIds,
        Guid accountId,
        CommitMessage message,
        DateTime createdAt)
    {
        Id = id;
        DocumentId = documentId;
        RootTreeId = rootTreeId;
        ParentIds = parentIds.Select(parentId => new CommitParent(parentId)).ToArray();
        AccountId = accountId;
        Message = message;
        CreatedAt = createdAt;
    }

    public static Commit Create(
        Guid documentId,
        string rootTreeId,
        IReadOnlyCollection<string> parentsId,
        Guid accountId,
        CommitMessage message)
    {
        var createdAt = DateTime.UtcNow;

        var commitIdBytes = Encoding.UTF8.GetBytes(
            $"{documentId:N}:{rootTreeId}:{accountId:N}:{message}:{createdAt.Ticks}");

        var commitId = Convert.ToHexStringLower(SHA256.HashData(commitIdBytes));

        return new Commit(
            commitId,
            documentId,
            rootTreeId,
            parentsId,
            accountId,
            message,
            createdAt);
    }
}
