using System.Text;

namespace DDLDumper;

public static class Dumper
{
    public static string ToSql(this Table table, bool indentation = true)
    {
        return Dump(table,GetColumns,GetPrimaryKey,GetForeignKeys,indentation);
    }

    public static string Dump(Table table,Func<List<Column>,string> getColumns,Func<Table,string>getPrimaryKey,Func<Table,string>getForeignKeys, bool indentation)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append
        (
$@"USE [{table.DbName}]
CREATE TABLE {table.TableName}
(
{(indentation ? getColumns(table.Properties).WithIndent() : GetColumns(table.Properties))}
)
{getPrimaryKey(table)}
{getForeignKeys(table)}");
        return sb.ToString();
    }

    public static Func<Table, string> GetPrimaryKey = table =>
    {
        var primaryKey = table.Properties.FirstOrDefault(x => x.IsPrimaryKey);
        var primaryKeyLine = $@"ALTER TABLE [dbo].[{table.TableName}] ADD CONSTRAINT [PK_{table.TableName}] PRIMARY KEY CLUSTERED ([{primaryKey.Name}])";
        return primaryKeyLine;
    };

    public static Func<List<Column>, string> GetColumns = columns =>
    {
        StringBuilder sb = new StringBuilder();
        foreach (var column in columns)
        {
            sb.Append($@"[{column.Name}] [{column.Type}] {(column.Nullable ? "NULL" : "NOT NULL")}{(column == columns.Last() ? "" : ",\r\n")}");
        }

        return sb.ToString();
    };

    public static Func<Table, string> GetForeignKeys = table =>
    {
        StringBuilder sb = new StringBuilder();
        foreach (var column in table.Properties.Where(x => x.IsForeignKey))
        {
            sb.Append($@"ALTER TABLE [dbo].[{table.TableName}] ADD CONSTRAINT [FK_{table.TableName}_{column.ForeignKeyTable.TableName}] FOREIGN KEY ([{column.Name}]) REFERENCES [dbo].[{column.ForeignKeyTable.TableName}] ([{column.ForeignKeyTable.Properties.FirstOrDefault(x => x.IsPrimaryKey).Name}])");
        }

        return sb.ToString();
    };
}