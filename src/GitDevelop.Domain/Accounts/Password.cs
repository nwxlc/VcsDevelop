namespace GitDevelop.Domain.Accounts;

public sealed class Password 
{
    public string HashedValue { get; private init; }
    private readonly string? _value;

    private Password(string hashedValue)
    {
        HashedValue = hashedValue;
    }

    private Password(string value, string hashedValue)
        : this(hashedValue)
    {
        _value = value;
    }

    public static Password Create(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        
        var hashedValue = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        return new Password(password, hashedValue);
    }
    
    internal bool Verify(Password passwordToVerify)
    {
        ArgumentNullException.ThrowIfNull(passwordToVerify);

        return BCrypt.Net.BCrypt.EnhancedVerify(passwordToVerify._value, HashedValue);
    }
}
