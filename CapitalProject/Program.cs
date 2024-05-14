using CapitalProject.Services;
using Microsoft.Azure.Cosmos;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Cosmos DB from appsettings.json
        builder.Services.AddSingleton<CosmosClient>(ConfigureCosmosClient(builder.Configuration));

        // Register services
        builder.Services.AddScoped<IQuestionService, QuestionService>();

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }

    private static CosmosClient ConfigureCosmosClient(IConfiguration configuration)
    {
        var cosmosDbConfig = configuration.GetSection("CosmosDb");
        var endpointUrl = cosmosDbConfig["EndpointUrl"];
        var primaryKey = cosmosDbConfig["PrimaryKey"];

        return new CosmosClient(endpointUrl, primaryKey);
    }
}