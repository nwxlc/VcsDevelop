using VcsDevelop.Core.Errors;

namespace VcsDevelop.Domain.Accounts.Errors;

public sealed class EmailAlreadyExists : Conflict
{
    public override string Message => "Email already exists";

    public override string BuildType()
        => "email-already-exists";
}
