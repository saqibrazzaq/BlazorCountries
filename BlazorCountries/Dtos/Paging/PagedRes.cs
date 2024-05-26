namespace BlazorCountries.Dtos.Paging
{
  public class PagedRes<T>
  {
    public int Count { get; set; }
    public List<T> Items { get; set; } = new List<T>();
  }
}
