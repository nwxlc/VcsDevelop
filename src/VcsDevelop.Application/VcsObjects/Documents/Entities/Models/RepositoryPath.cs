namespace VcsDevelop.Application.VcsObjects.Documents.Entities.Models;

public readonly record struct RepositoryPath
{
    private const int MaxLength = 255;
    private static readonly char[] InvalidChars = ['\0', '<', '>', ':', '"', '|', '?', '*'];

    public string Value { get; }

    private RepositoryPath(string value) => Value = value;

    public static RepositoryPath Create(string? path, string fileName)
    {
        var fileNameClean = fileName.Trim();
        var normalizedBase = (path ?? string.Empty).Replace('\\', '/').Trim().Trim('/');

        var fullPath = string.IsNullOrEmpty(normalizedBase)
            ? fileNameClean
            : $"{normalizedBase}/{fileNameClean}";

        Validate(fullPath);
        return new RepositoryPath(fullPath);
    }

    private static void Validate(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path.Length > MaxLength)
        {
            throw new ArgumentException("Invalid path length.");
        }

        var span = path.AsSpan();
        int start = 0;
        while (start < span.Length)
        {
            int end = span[start..].IndexOf('/');
            var segment = end == -1 ? span[start..] : span.Slice(start, end);

            if (segment.IsEmpty || segment.SequenceEqual(".") || segment.SequenceEqual(".."))
            {
                throw new ArgumentException("Path contains invalid segments.");
            }

            if (segment.IndexOfAny(InvalidChars) != -1)
            {
                throw new ArgumentException("Path contains invalid characters.");
            }

            if (end == -1) break;
            start += end + 1;
        }
    }

    public override string ToString() => Value;
}
