using BlazorCountries.Utility;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlazorCountries.Entities
{
  public class Person
  {
    public string id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    public string firstName { get; set; } = string.Empty;
    [Required]
    public string lastName { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string email { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public string userId { get; set; } = Constants.PartitionKey;
    public IList<Address> addresses { get; set; } = new List<Address>();
  }
}
