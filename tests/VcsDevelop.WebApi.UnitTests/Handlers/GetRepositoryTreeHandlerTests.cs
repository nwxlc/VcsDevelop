using FluentAssertions;
using Moq;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Application.VcsObjects.Documents.QueryHandlers;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects;
using Xunit;
using Branch = VcsDevelop.Domain.VcsObjects.Branch;
using Commit = VcsDevelop.Domain.VcsObjects.Commit;

namespace VcsDevelop.WebApi.UnitTests.Handlers;

public sealed class GetRepositoryTreeHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldPopulateLastCommitMessage_ForFilesAndDirectories()
    {
        var accountId = Guid.NewGuid();
        var document = Document.Create(
            accountId,
            "Repo",
            "main",
            DocumentMetadata.Create("Repo", null, null));

        var rootTree = Tree.Create(
            "tree-1",
            [
                new TreeEntry("docs/readme.md", "blob-docs"),
                new TreeEntry("src/app.cs", "blob-old")
            ]);

        var headTree = Tree.Create(
            "tree-2",
            [
                new TreeEntry("docs/readme.md", "blob-docs"),
                new TreeEntry("src/app.cs", "blob-new")
            ]);

        var rootCommit = new Commit(
            "commit-root",
            document.Id,
            rootTree.Id,
            [],
            accountId,
            CommitMessage.Create("initial import"),
            new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc));

        var headCommit = new Commit(
            "commit-head",
            document.Id,
            headTree.Id,
            [rootCommit.Id],
            accountId,
            CommitMessage.Create("update app"),
            new DateTime(2026, 1, 2, 10, 0, 0, DateTimeKind.Utc));

        var branch = Branch.Create(document.Id, document.DefaultBranchName, headCommit.Id);

        var documentRepository = new Mock<IDocumentRepository>();
        documentRepository
            .Setup(repository => repository.FindByIdAsync(document.Id, accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        var branchRepository = new Mock<IBranchRepository>();
        branchRepository
            .Setup(repository => repository.FindByDocumentAndNameAsync(document.Id, document.DefaultBranchName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);

        var commitRepository = new Mock<ICommitRepository>();
        commitRepository
            .Setup(repository => repository.FindByIdAsync(headCommit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(headCommit);
        commitRepository
            .Setup(repository => repository.FindByIdAsync(rootCommit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rootCommit);

        var treeRepository = new Mock<ITreeRepository>();
        treeRepository
            .Setup(repository => repository.FindByIdAsync(headTree.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(headTree);
        treeRepository
            .Setup(repository => repository.FindByIdAsync(rootTree.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rootTree);

        var requestContext = new Mock<IRequestContext>();
        requestContext.Setup(context => context.GetRequiredAccountId()).Returns(accountId);

        var handler = new GetRepositoryTreeHandler(
            documentRepository.Object,
            branchRepository.Object,
            commitRepository.Object,
            treeRepository.Object,
            requestContext.Object);

        var rootResponse = await handler.HandleAsync(
            GetRepositoryTreeQuery.Create(document.Id, null),
            CancellationToken.None);

        rootResponse.Entries.Should().ContainSingle(entry => entry.Path == "docs" && entry.LastCommitMessage == "initial import");
        rootResponse.Entries.Should().ContainSingle(entry => entry.Path == "src" && entry.LastCommitMessage == "update app");

        var srcResponse = await handler.HandleAsync(
            GetRepositoryTreeQuery.Create(document.Id, "src"),
            CancellationToken.None);

        srcResponse.Entries.Should().ContainSingle(entry =>
            entry.Name == "app.cs" &&
            entry.Type == "file" &&
            entry.LastCommitMessage == "update app");
    }
}
