using System.Diagnostics;
using System.Net.WebSockets;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using NUnit.Framework;

namespace DDLDumper;

[TestFixture]
public class Test
{
    Table table = new Table()
    {
        DbName = "College",
        TableName = "Lesson",
        Properties = new List<Column>()
        {
            new Column
            {
                Name = "Id",
                Type = "bigint",
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new Column
            {
                Name = "Title",
                Type = "nvarchar"
            },
            new Column
            {
                Name = "UnitNum",
                Type = "bigint"
            },
            new Column
            {
                Name = "MasterId",
                Type = "bigint",
                IsForeignKey = true,
                ForeignKeyTable = new Table()
                {
                    DbName = "College",
                    TableName = "Master",
                    Properties = new List<Column>()
                    {
                        new Column
                        {
                            Name = "Id",
                            Type = "bigint",
                            IsIdentity = true,
                            IsPrimaryKey = true
                        }
                    }
                }
            }
        }
    };

    [Test]
    public void ObjectIntoSql()
    {
        var projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
        string expectedPath = $@"{projectPath}\DDlDumper\Expected\Expected.txt";
        string actualPath = $@"{projectPath}\DDlDumper\Actual\Actual.txt";
        Dumper dm = new Dumper(table);
        File.WriteAllText(actualPath, dm.Dump());
        var expected = new MemoryStream(File.ReadAllBytes(expectedPath));
        var actual = new MemoryStream(File.ReadAllBytes(actualPath));
        Process.Start("cmd.exe", $"/C start bcompare.exe {expectedPath} {actualPath}");
        FileAssert.AreEqual(expected, actual);
    }
}