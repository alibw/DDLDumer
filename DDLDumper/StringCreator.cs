using System.Text;

namespace DDLDumper;

public static class StringCreator
{
    private static StringBuilder _sb = new StringBuilder();

    public static void Indent(int count = 2)
    {
        for (int i = 0; i < count; i++)
        {
            _sb.Append(' ');
        }
    }

    public static void Unindent(int count = 2)
    {
        _sb.Remove(0, count);
    }

    public static string WithIndent(this string input)
    {
        StringBuilder sb = new StringBuilder();
        using (StringReader reader = new StringReader(input))
        {
            var splittedLines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in splittedLines)
            {
                Indent();
                sb.Append($@"{_sb.ToString()}{line}{(line == splittedLines.Last() ? "" : "\r\n")}");
                Unindent();
            }
        }

        return sb.ToString();
    }
}