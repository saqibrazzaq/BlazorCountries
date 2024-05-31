using BlazorCountries.Dtos.Paging;
using BlazorCountries.Dtos;
using BlazorCountries.Entities;
using Microsoft.Azure.Cosmos;
using System.Linq;
using BlazorCountries.Utility;

namespace BlazorCountries.Services
{
  public interface IStateService
  {
    State? Get(string id);
    Task<State> AddAsync(State? item);
    Task<State> UpdateAsync(State? item);
    Task DeleteAsync(State? item);
    Task Reset();
    PagedRes<State> Search(StateSearchReq dto);
  }

  public class StateService : IStateService
  {
    private Container _container;
    public StateService(
        CosmosClient cosmosDbClient,
        string databaseName,
        string containerName)
    {
      _container = cosmosDbClient.GetContainer(databaseName, containerName);
    }
    public async Task<State> AddAsync(State? item)
    {
      if (item is null) throw new Exception("Item is null");

      return await _container.CreateItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
    public async Task DeleteAsync(State? item)
    {
      if (item is null) throw new Exception("Item is null");

      var itemFind = Get(item.id);
      if (itemFind is null) return;

      await _container.DeleteItemAsync<State>(item.id, new PartitionKey(Constants.PartitionKey));
    }
    public async Task Reset()
    {
      await _container.DeleteContainerAsync();
      await _container.Database.CreateContainerIfNotExistsAsync(Constants.StateCt, "/userId");
    }
    public State? Get(string id)
    {
      try
      {
        IQueryable<State> queryable = _container.GetItemLinqQueryable<State>(true);
        queryable = queryable.Where(item => item.id == id);
        return queryable.ToArray().FirstOrDefault();
      }
      catch (CosmosException) //For handling item not found and other exceptions
      {
        return null;
      }
    }
    public PagedRes<State> Search(StateSearchReq dto)
    {
      try
      {
        IQueryable<State> queryable = _container.GetItemLinqQueryable<State>(true);

        if (!string.IsNullOrEmpty(dto.CountryId))
        {
          queryable = queryable.Where(x => x.countryId == dto.CountryId);
        }

        if (!string.IsNullOrEmpty(dto.CountryName))
        {
          queryable = queryable.Where(x => x.countryName == dto.CountryName);
        }

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

        return new PagedRes<State> { Items = items, Count = count };
      }
      catch (CosmosException) //For handling item not found and other exceptions
      {
        return new PagedRes<State>();
      }
    }
    public async Task<State> UpdateAsync(State? item)
    {
      if (item is null) throw new Exception("Item is null");

      var itemFind = Get(item.id);
      if (itemFind is null) throw new Exception("Cannot update.");

      return await _container.UpsertItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
  }
}
