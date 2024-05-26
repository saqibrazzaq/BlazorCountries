using BlazorCountries.Dtos.Paging;

namespace BlazorCountries.Dtos
{
  public class PersonSearchReq : PagedReq
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
  }
}
