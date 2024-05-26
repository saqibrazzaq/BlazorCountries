using BlazorCountries.Dtos.Paging;
using BlazorCountries.Dtos;
using BlazorCountries.Entities;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Azure.Cosmos;
using BlazorCountries.Utility;
using System.Collections;

namespace BlazorCountries.Services
{
  public interface ICountryService
  {
    Country? Get(string id);
    Task<Country> AddAsync(Country? item);
    Task<Country> UpdateAsync(Country? item);
    Task DeleteAsync(Country? item);
    Task Reset();
    PagedRes<Country> Search(CountrySearchReq dto);
  }

  public class CountryService : ICountryService
  {
    private Container _container;
    public CountryService(
        CosmosClient cosmosDbClient,
        string databaseName,
        string containerName)
    {
      _container = cosmosDbClient.GetContainer(databaseName, containerName);
    }
    public async Task<Country> AddAsync(Country? item)
    {
      if (item is null) throw new Exception("Item is null");

      return await _container.CreateItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
    public async Task DeleteAsync(Country? item)
    {
      if (item is null) throw new Exception("Item is null");

      var itemFind = Get(item.id);
      if (itemFind is null) return;

      await _container.DeleteItemAsync<Country>(item.id, new PartitionKey(Constants.PartitionKey));
    }
    public async Task Reset()
    {
      await _container.DeleteContainerAsync();
      await _container.Database.CreateContainerIfNotExistsAsync(Constants.CountryCt, "/userId");
    }
    public Country? Get(string id)
    {
      try
      {
        IQueryable<Country> queryable = _container.GetItemLinqQueryable<Country>(true);
        queryable = queryable.Where(item => item.id == id);
        return queryable.ToArray().FirstOrDefault();
      }
      catch (CosmosException) //For handling item not found and other exceptions
      {
        return null;
      }
    }
    public PagedRes<Country> Search(CountrySearchReq dto)
    {
      try
      {
        IQueryable<Country> queryable = _container.GetItemLinqQueryable<Country>(true);

        if (!string.IsNullOrEmpty(dto.Name))
        {
          queryable = queryable.Where(x => x.name.Contains(dto.Name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(dto.Code))
        {
          queryable = queryable.Where(x => x.code.Contains(dto.Code, StringComparison.OrdinalIgnoreCase));
        }

        // Count before paging
        var count = queryable.Count();

        // Sort
        if (string.IsNullOrEmpty(dto.OrderBy))
          queryable = queryable.OrderBy(x => x.name);
        else if (dto.OrderBy.Equals("name", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Ascending)
          queryable = queryable.OrderBy(x => x.name);
        else if (dto.OrderBy.Equals("name", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Descending)
          queryable = queryable.OrderByDescending(x => x.name);
        else if (dto.OrderBy.Equals("code", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Ascending)
          queryable = queryable.OrderBy(x => x.code);
        else if (dto.OrderBy.Equals("code", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Descending)
          queryable = queryable.OrderByDescending(x => x.code);
        else
          queryable = queryable.OrderBy(x => x.name);

        // Get list after paging
        queryable = queryable
            .Skip(dto.Skip)
            .Take(dto.Take);
        var items = queryable.ToList();

        return new PagedRes<Country> { Items = items, Count = count };
      }
      catch (CosmosException) //For handling item not found and other exceptions
      {
        return new PagedRes<Country>();
      }
    }
    public async Task<Country> UpdateAsync(Country? item)
    {
      if (item is null) throw new Exception("Item is null");

      var itemFind = Get(item.id);
      if (itemFind is null) throw new Exception("Cannot update.");

      return await _container.UpsertItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
  }
}
