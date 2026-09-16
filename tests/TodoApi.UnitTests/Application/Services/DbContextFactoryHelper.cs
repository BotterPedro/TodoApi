using Microsoft.EntityFrameworkCore;
using TodoApi.Infrastructure.Data;

namespace TodoApi.UnitTests.Application.Services;

public static class DbContextFactoryHelper
{
    public static AppDbContext Criar()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}