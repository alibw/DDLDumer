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
        var expected = @"CREATE TABLE Lesson
(
  [Id] [bigint] NOT NULL,
  [Title] [nvarchar] NOT NULL,
  [UnitNum] [bigint] NOT NULL,
  [MasterId] [bigint] NOT NULL
)
ALTER TABLE [dbo].[Lesson] ADD CONSTRAINT [PK_Lesson] PRIMARY KEY CLUSTERED ([Id])
ALTER TABLE [dbo].[Lesson] ADD CONSTRAINT [FK_Lesson_Master] FOREIGN KEY ([MasterId]) REFERENCES [dbo].[Master] ([Id])";
        
        Dumper dm = new Dumper(table);

        Assert.AreEqual(expected,dm.Dump());
    }
}