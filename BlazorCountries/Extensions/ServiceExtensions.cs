using BlazorCountries.Services;
using BlazorCountries.Utility;
using dotenv.net;

namespace BlazorCountries.Extensions
{
  public static class ServiceExtensions
  {
    private static string? CosmosDb => Environment.GetEnvironmentVariable("CosmosDb");
    private static string? CosmosAccount => Environment.GetEnvironmentVariable("CosmosAccount");
    private static string? CosmosKey => Environment.GetEnvironmentVariable("CosmosKey");

    public static void LoadEnvironmentVariables(this IServiceCollection services)
    {
      DotEnv.Load();
    }
    public static void ConfigureCosmosDb(this IServiceCollection services,
        IConfiguration config)
    {
      services.AddSingleton<ICountryService>(InitCountry(
          config).GetAwaiter().GetResult());
      services.AddSingleton<IStateService>(InitState(
          config).GetAwaiter().GetResult());
      services.AddSingleton<IPersonService>(InitPerson(
          config).GetAwaiter().GetResult());
    }
    private static async Task<CountryService> InitCountry(IConfiguration configuration)
    {
      var client = new Microsoft.Azure.Cosmos.CosmosClient(CosmosAccount, CosmosKey);
      var database = await client.CreateDatabaseIfNotExistsAsync(CosmosDb);
      await database.Database.CreateContainerIfNotExistsAsync(Constants.CountryCt, "/userId");

      var countryService = new CountryService(client, CosmosDb ?? "", Constants.CountryCt ?? "");
      return countryService;
    }
    private static async Task<StateService> InitState(IConfiguration configuration)
    {
      var client = new Microsoft.Azure.Cosmos.CosmosClient(CosmosAccount, CosmosKey);
      var database = await client.CreateDatabaseIfNotExistsAsync(CosmosDb);
      await database.Database.CreateContainerIfNotExistsAsync(Constants.StateCt, "/userId");

      var stateService = new StateService(client, CosmosDb ?? "", Constants.StateCt ?? "");
      return stateService;
    }

    private static async Task<PersonService> InitPerson(IConfiguration configuration)
    {
      var client = new Microsoft.Azure.Cosmos.CosmosClient(CosmosAccount, CosmosKey);
      var database = await client.CreateDatabaseIfNotExistsAsync(CosmosDb);
      await database.Database.CreateContainerIfNotExistsAsync(Constants.PersonCt, "/userId");

      var personService = new PersonService(client, CosmosDb ?? "", Constants.PersonCt ?? "");
      return personService;
    }
  }
}
