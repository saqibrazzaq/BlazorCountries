using BlazorCountries.Dtos.Paging;

namespace BlazorCountries.Dtos
{
  public class CountrySearchReq : PagedReq
  {
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
  }
}
