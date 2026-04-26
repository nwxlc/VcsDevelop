using VcsDevelop.Application.Accounts.Entities.Models;
using VcsDevelop.Application.Accounts.Entities.Queries;
using VcsDevelop.Core.Application;

namespace VcsDevelop.Application.Accounts.Abstractions;

public interface IGetAccountByIdHandler : IHandler<GetAccountByIdQuery, AccountInfoResponse>;
