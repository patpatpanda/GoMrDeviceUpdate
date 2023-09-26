using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureFunctionDevice.Models;

namespace AzureFunctionDevice.Data
{
	public class AzureDbContext : DbContext
	{
		private readonly IConfiguration _configuration;

		public AzureDbContext(DbContextOptions<AzureDbContext> dbContextOptions) : base(dbContextOptions)
		{

		}



		public DbSet<DeviceStatus> DeviceStatusSet { get; set; }
		




	}
}
