using System.Text;

namespace DDLDumper;

public static class StringCreator
{
    private static StringBuilder _sb;

    public static string Indent(int count = 2)
    {
        _sb = new StringBuilder();
        for (int i = 0; i <= count; i++)
        {
            _sb.Append(' ');
        }

        return _sb.ToString();
    }

    public static string Unindent(int count = 3)
    {
        _sb.Remove(0, count);
        return _sb.ToString();
    }

    public static string WithIndent(this string input)
    {
        StringBuilder sb = new StringBuilder();
        using (StringReader reader = new StringReader(input))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                sb.AppendLine($@"{Indent()}{line}");
            }
        }

        return sb.ToString();
    }
}