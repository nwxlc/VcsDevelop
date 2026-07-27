using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.Accounts.Commands;

namespace VcsDevelop.Application.Accounts.CommandHandlers;

public sealed class UpdateAccountHandler : IUpdateAccountHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IRequestContext _requestContext;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountHandler(
        IAccountRepository accountRepository,
        IRequestContext requestContext,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(accountRepository);
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _accountRepository = accountRepository;
        _requestContext = requestContext;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        if (command.Id != _requestContext.GetRequiredAccountId())
        {
            throw new Exception("You are not authorized to update this account");
        }

        var account = await _accountRepository.FindByIdAsync(command.Id, cancellationToken)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw new KeyNotFoundException($"Account {command.Id} not found");
        }

        account.Update(command.Name, command.Bio, command.AvatarUrl);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
