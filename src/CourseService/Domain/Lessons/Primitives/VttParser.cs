using System.Globalization;
using System.Text.RegularExpressions;

namespace Courses.Domain.Lessons.Primitives;

public static class VttParser
{
    private static readonly string[] _blockSeparators = { "\r\n\r\n", "\n\n" };
    private static readonly string[] _lineSeparators = { "\r\n", "\n" };
    private static readonly string[] _timeArrowSeparator = { "-->" };

    public static List<TranscriptLine> Parse(string vttContent)
    {
        var result = new List<TranscriptLine>();
        if (string.IsNullOrWhiteSpace(vttContent))
        {
            return result;
        }

        string contentWithoutHeader = Regex.Replace(vttContent, @"^WEBVTT.*\n", "", RegexOptions.IgnoreCase);

        string[] blocks = contentWithoutHeader.Split(_blockSeparators, StringSplitOptions.RemoveEmptyEntries);

        foreach (string block in blocks)
        {
            string[] lines = block.Split(_lineSeparators, StringSplitOptions.RemoveEmptyEntries);

            string? timeLine = lines.FirstOrDefault(l => l.Contains("-->", StringComparison.Ordinal));

            if (timeLine == null)
            {
                continue;
            }

            string[] times = timeLine.Split(_timeArrowSeparator, StringSplitOptions.None);
            if (times.Length != 2)
            {
                continue;
            }

            int timeLineIndex = Array.IndexOf(lines, timeLine);
            string text = string.Join(" ", lines.Skip(timeLineIndex + 1)).Trim();

            text = Regex.Replace(text, "<.*?>", string.Empty);

            if (TryParseVttTime(times[0].Trim(), out TimeSpan start) &&
                TryParseVttTime(times[1].Trim(), out TimeSpan end))
            {
                result.Add(new TranscriptLine(start, end, text));
            }
        }
        return result;
    }

    private static bool TryParseVttTime(string timeString, out TimeSpan result)
    {
        return TimeSpan.TryParse(timeString.Replace(',', '.'), CultureInfo.InvariantCulture, out result);
    }
}
