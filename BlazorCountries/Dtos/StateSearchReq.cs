using BlazorCountries.Dtos.Paging;

namespace BlazorCountries.Dtos
{
  public class StateSearchReq : PagedReq
  {
    public string CountryId { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
  }
}
