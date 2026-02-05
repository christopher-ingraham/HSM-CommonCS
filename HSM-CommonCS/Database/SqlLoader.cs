namespace HSM_CommonCS.Database;

public static class SqlLoader
{
    public static string Load(string relativePath)
    {
        // Example path:
        // "LaminarCooling/AccStatus_ByBank.sql"

        var baseDir = AppContext.BaseDirectory;
        var sqlRoot = Path.Combine(baseDir, "Sql");

        var fullPath = Path.Combine(sqlRoot, relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"SQL file not found: {fullPath}");

        return File.ReadAllText(fullPath);
    }
}
