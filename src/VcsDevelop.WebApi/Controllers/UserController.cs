using Microsoft.AspNetCore.Mvc;
using VcsDevelop.Application.Accounts.Abstractions;
using VcsDevelop.Application.Accounts.Entities;
using VcsDevelop.Domain.Accounts;
using VcsDevelop.Domain.Accounts.Commands;
using VcsDevelop.WebApi.Contracts;
using LoginRequest = VcsDevelop.WebApi.Contracts.LoginRequest;

namespace VcsDevelop.WebApi.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    [HttpPost("registration")]
    public async Task<ActionResult<AccountResponse>> RegistrationAsync(
        [FromBody]RegistrationRequest request,
        [FromServices]IRegistrationCommandHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(handler);

        var command = RegistrationCommand.Create(
            request.Name,
            request.Email,
            Password.Create(request.Password));

        var accountResponse = await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        return Ok(accountResponse);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccountResponse>> LoginAsync(
        [FromBody]LoginRequest request,
        [FromServices]ILoginCommandHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(handler);

        var command = LoginCommand.Create(
            request.Email,
            Password.Create(request.Password));

        var accountResponse = await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        return Ok(accountResponse);
    }
}
