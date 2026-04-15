using VcsDevelop.Application.Accounts.Entities;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.Accounts.Commands;

namespace VcsDevelop.Application.Accounts.Abstractions;

public interface ILoginCommandHandler : IHandler<LoginCommand, AccountResponse>;
