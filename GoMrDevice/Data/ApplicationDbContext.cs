using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoMrDevice.Models;

namespace GoMrDevice.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{

		}

		public ApplicationDbContext()
		{

		}
		public DbSet<RemovedDevice> RemovedDevices { get; set; }
		


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(@"Server=.;Database=GoMrDeviceDb;Trusted_Connection=True;TrustServerCertificate=true;");
			}
		}
	}
}
