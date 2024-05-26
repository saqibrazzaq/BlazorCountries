using BlazorCountries.Utility;

namespace BlazorCountries.Entities
{
  public class State
  {
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string name { get; set; } = string.Empty;
    public string code { get; set; } = string.Empty;
    public string countryId { get; set; } = string.Empty;
    public string countryName { get; set; } = string.Empty;
    public string userId { get; set; } = Constants.PartitionKey;
  }
}
