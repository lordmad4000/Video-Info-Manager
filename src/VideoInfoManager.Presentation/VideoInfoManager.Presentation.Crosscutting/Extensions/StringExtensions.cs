using System.Text.RegularExpressions;

namespace VideoInfoManager.Presentation.Crosscutting.Extensions;

public static class StringExtensions
{
    public static string RemoveNewLine(this string str) => Regex.Replace(str, @"\t|\n|\r", "");
}
