using BlazorCountries.Utility;
using System.ComponentModel.DataAnnotations;

namespace BlazorCountries.Entities
{
  public class Country
  {
    public string id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    public string name { get; set; } = string.Empty;
    [Required, MaxLength(3)]
    public string code { get; set; } = string.Empty;
    public string userId { get; set; } = Constants.PartitionKey;
  }
}
