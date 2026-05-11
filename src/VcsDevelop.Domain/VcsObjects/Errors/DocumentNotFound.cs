using VcsDevelop.Core.Errors;

namespace VcsDevelop.Domain.VcsObjects.Errors;

public sealed class DocumentNotFound : NotFound
{
    public Guid DocumentId { get; }
    public override string Message => "Document not found.";

    public DocumentNotFound(Guid documentId)
    {
        DocumentId = documentId;
    }
}
