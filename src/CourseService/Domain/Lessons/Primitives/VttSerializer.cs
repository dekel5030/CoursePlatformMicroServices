using System.Globalization;
using System.Text;

namespace Courses.Domain.Lessons.Primitives;

/// <summary>
/// Serializes transcript lines to WEBVTT format.
/// </summary>
public static class VttSerializer
{
    private const string WebVttHeader = "WEBVTT";
    private const string TimeFormat = @"hh\:mm\:ss\.fff";

    public static string Serialize(IReadOnlyList<TranscriptLine> lines)
    {
        if (lines is null || lines.Count == 0)
        {
            return WebVttHeader + "\n\n";
        }

        var sb = new StringBuilder();
        sb.AppendLine(WebVttHeader);
        sb.AppendLine();

        foreach (TranscriptLine line in lines)
        {
            string start = FormatVttTime(line.Start);
            string end = FormatVttTime(line.End);

            sb.Append(start).Append(" --> ").AppendLine(end);

            string text = line.Text?.Replace("\r\n", "\n", StringComparison.Ordinal) ?? string.Empty;
            sb.AppendLine(text);

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string FormatVttTime(TimeSpan time)
    {
        return time.ToString(TimeFormat, CultureInfo.InvariantCulture);
    }
}
