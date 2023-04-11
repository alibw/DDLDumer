using System.Diagnostics;
using System.Net.WebSockets;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using NUnit.Framework;
using SQLParser;
using DDLDumper;

namespace DDLDumper;

[TestFixture]
public class Test
{
    string script = @"
CREATE TABLE [dbo].[Session]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[MasterId] [bigint] NOT NULL,
[WeekDay] [nvarchar] NOT NULL,
[Time] [time] NOT NULL,
[ClassNum] [nvarchar] NOT NULL,
[LessonId] [bigint] NOT NULL
)
ALTER TABLE [dbo].[Session] ADD CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED ([Id])
ALTER TABLE [dbo].[Session] ADD CONSTRAINT [FK_Session_Lesson] FOREIGN KEY ([LessonId]) REFERENCES [dbo].[Lesson] ([Id])
ALTER TABLE [dbo].[Session] ADD CONSTRAINT [FK_Session_Master] FOREIGN KEY ([MasterId]) REFERENCES [dbo].[Master] ([Id])";
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
        List<Column> columns = new List<Column>();
        var parsedTable = Parser.Parse(script);
        foreach (var property in parsedTable.Properties)
        {
            var column = new Column()
            {
                Name = property.Name,
                Type = property.Type,
                IsIdentity = property.IsIdentity,
                IsPrimaryKey = property.IsPrimaryKey,
                Nullable = property.Nullable,
                IsForeignKey = property.IsForeignKey,
                ForeignKeyTableName = property.ForeignKeyTableName
            };
            columns.Add(column);
        }
        var table2 = new Table()
        {
            Properties = columns,
            TableName = parsedTable.TableName,
            DbName = parsedTable.DbName
        };

        Dumper dm = new Dumper(table2);
        
        var dumpedSql = dm.Dump();
        Assert.AreEqual(table,parsedTable);
    }
}