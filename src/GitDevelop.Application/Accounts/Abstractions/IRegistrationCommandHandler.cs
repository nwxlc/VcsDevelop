using GitDevelop.Application.Accounts.Entities;
using GitDevelop.Core.Application;
using GitDevelop.Domain.Accounts.Commands;

namespace GitDevelop.Application.Accounts.Abstractions;

public interface IRegistrationCommandHandler : IHandler<RegistrationCommand, AccountResponse>;
