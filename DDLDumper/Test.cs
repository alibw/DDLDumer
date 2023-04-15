 using System.Diagnostics;
using System.Net.WebSockets;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using NUnit.Framework;
 using SQLParser;

 namespace DDLDumper;

[TestFixture]
public class Test
{
    
    public static string script = @"
CREATE TABLE [dbo].[Session]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[MasterId] [bigint] NOT NULL,
[WeekDay] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Time] [time] NOT NULL,
[ClassNum] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[LessonId] [bigint] NOT NULL
)
GO
ALTER TABLE [dbo].[Session] ADD CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED ([Id])
GO
ALTER TABLE [dbo].[Session] ADD CONSTRAINT [FK_Session_Lesson] FOREIGN KEY ([LessonId]) REFERENCES [dbo].[Lesson] ([Id])
GO
ALTER TABLE [dbo].[Session] ADD CONSTRAINT [FK_Session_Master] FOREIGN KEY ([MasterId]) REFERENCES [dbo].[Master] ([Id])
GO
";
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

    [Test]
    public void CycleTest()
    {
        var parsedTable = Parser.Parse(script);

        var columns = new List<Column>();
        foreach (var item in parsedTable.Properties)
        {
            var column = new Column()
            {
                Name = item.Name,
                Type = item.Type,
                IsIdentity = item.IsIdentity,
                IsForeignKey = item.IsForeignKey,
                Nullable = item.Nullable,
                IsPrimaryKey = item.IsPrimaryKey,
                ForeignKeyTableName = item.ForeignKeyTableName
            };
            columns.Add(column);
        }
        var convertedTable = new Table()
        {
            TableName = parsedTable.TableName,
            DbName = parsedTable.DbName,
            Properties = columns
        };
        Dumper dm = new Dumper(convertedTable);
        Console.WriteLine(dm.Dump());
    }
}