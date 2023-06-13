using Newtonsoft.Json.Serialization;
using Microsoft.ApplicationInsights.AspNetCore;
using AzureAPI.Helper;
using AzureAPI.IHelper;

namespace AzureAPI
{
    public class Program
    {
        public static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfiguration configurationsection)
        {
            var databaseName = configurationsection["DatabaseName"];
            var containerName = configurationsection["ContainerName"];
            var account = configurationsection["Account"];
            var key = configurationsection["Key"];
            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            var container = await database.Database.CreateContainerIfNotExistsAsync(containerName,"/type");
            var cosmosDbService = new CosmosDbService(client,databaseName,containerName);
            return cosmosDbService;

        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
            
            builder.Services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IIotHubService, IotHubService>();
          
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(options =>
            {
                options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();


            app.MapControllers();
            

            app.Run();
        }
    }
}