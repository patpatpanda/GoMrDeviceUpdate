using AzureFunctionDevice.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureAppConfiguration(config => config.AddJsonFile("local.settings.json"))
	.ConfigureServices((builder, services) =>
	{
		services.AddDbContext<AzureDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


	})
	.Build();

host.Run();