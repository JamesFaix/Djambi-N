using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Apex.Api.Db.Model
{
    public class ApexDbContextFactory : IDesignTimeDbContextFactory<ApexDbContext>
    {
        public ApexDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables("APEX_");

            var config = builder.Build();

            var connStr = config.GetValue<string>("Sql:ConnectionString");

            var optionsBuilder = new DbContextOptionsBuilder<ApexDbContext>();
            optionsBuilder.UseMySql(connStr);

            return new ApexDbContext(optionsBuilder.Options);
        }
    }
}
