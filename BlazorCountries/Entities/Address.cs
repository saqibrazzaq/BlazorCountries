using BlazorCountries.Utility;

namespace BlazorCountries.Entities
{
  public class Address
  {
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string? line1 { get; set; } = string.Empty;
    public string? line2 { get; set; } = string.Empty;
    public string? city { get; set; } = string.Empty;
    public string? state { get; set; } = string.Empty;
    public string? country { get; set; } = string.Empty;
    public bool isPrimary { get; set; } = false;
    public string? userId { get; set; } = Constants.PartitionKey;
  }
}
