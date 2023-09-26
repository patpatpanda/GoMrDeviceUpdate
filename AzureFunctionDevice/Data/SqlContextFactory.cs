using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionDevice.Data
{
	public class SqlContextFactory : IDesignTimeDbContextFactory<AzureDbContext>
	{
		private readonly IConfiguration _configuration;

		public SqlContextFactory(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public SqlContextFactory()
		{

		}

		public AzureDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<AzureDbContext>();
			optionsBuilder.UseSqlServer(
				"Server=localhost;Database=DeviceDatabase; TrustServerCertificate=true; Trusted_Connection=True;MultipleActiveResultSets=true");

			return new AzureDbContext(optionsBuilder.Options);
		}
	}
}
