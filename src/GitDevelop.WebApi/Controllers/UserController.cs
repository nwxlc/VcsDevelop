using GitDevelop.Application.Accounts.Abstractions;
using GitDevelop.Application.Accounts.Entities;
using GitDevelop.Domain.Accounts;
using GitDevelop.Domain.Accounts.Commands;
using GitDevelop.WebApi.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GitDevelop.WebApi.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    [HttpPost("registration")]
    public async Task<ActionResult<AccountResponse>> RegistrationAsync(
        [FromBody] RegistrationRequest request,
        [FromServices] IRegistrationCommandHandler handler,
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
}
