using CarAgence.Data;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Tests.Helpers;

public static class TestHelper
{
    public static AppDbContext CreateInMemoryContext(string? dbName = null)
    {
        dbName ??= Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }
}
