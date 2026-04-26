using VcsDevelop.Core.Application;
using VcsDevelop.Domain.Accounts.Commands;

namespace VcsDevelop.Application.Accounts.Abstractions;

public interface IUpdateAccountHandler : IHandler<UpdateAccountCommand>;
