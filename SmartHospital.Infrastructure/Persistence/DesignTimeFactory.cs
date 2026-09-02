using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartHospital.Infrastructure.Persistence;

public class DesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=smarthospital;Username=postgres;Password=postgres")
            .Options;
        return new ApplicationDbContext(options);
    }
}
