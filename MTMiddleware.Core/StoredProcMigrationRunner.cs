using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;

//StoredProcMigrationRunner.Exec(Assembly.GetExecutingAssembly(), migrationBuilder);

namespace MTMiddleware.Core;

public static class StoredProcMigrationRunner
{
    public static void Exec(Assembly assembly, MigrationBuilder migrationBuilder)
    {
        var sqlFiles = assembly.GetManifestResourceNames().
            Where(file => file.EndsWith(".sql"));
        foreach (var sqlFile in sqlFiles)
        {
            using (Stream stream = assembly.GetManifestResourceStream(sqlFile))
            using (StreamReader reader = new StreamReader(stream))
            {
                var fileContent = reader.ReadToEnd();
                fileContent = fileContent.Replace("'", "''");
                var sqlScript = $"EXEC(N'{fileContent}')";

                migrationBuilder.Sql(sqlScript);
            }
        }
    }
}
