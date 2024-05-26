using BlazorCountries.Utility;

namespace BlazorCountries.Dtos.Paging
{
  public class PagedReq
  {
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = Constants.PageSize;
    public string? OrderBy { get; set; }
    public string? SortOrder { get; set; } = Constants.Ascending;
    public string? SearchText { get; set; }
  }
}
