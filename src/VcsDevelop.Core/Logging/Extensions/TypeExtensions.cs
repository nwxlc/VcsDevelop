using System.Text;

namespace VcsDevelop.Core.Logging.Extensions;

public static class TypeExtensions
{
    public static string DisplayName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var builder = new StringBuilder();
        ProcessDisplayName(type, builder);
        return builder.ToString();
    }

    private static void ProcessDisplayName(Type type, StringBuilder builder)
    {
        var genericPartIndex = type.Name.IndexOf('`', StringComparison.InvariantCultureIgnoreCase);

        if (genericPartIndex <= 0)
        {
            builder.Append(type.Name);
            return;
        }

        builder.Append(type.Name, 0, genericPartIndex);
        builder.Append('<');

        var genericArguments = type.GetGenericArguments();

        for (var index = 0; index < genericArguments.Length; index++)
        {
            var genericArgument = genericArguments[index];
            ProcessDisplayName(genericArgument, builder);

            if (index < genericArguments.Length - 1)
            {
                builder.Append(',');
            }
        }

        builder.Append('>');
    }
}
