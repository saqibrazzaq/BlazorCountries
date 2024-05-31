using BlazorCountries.Dtos.Paging;
using BlazorCountries.Dtos;
using Microsoft.Azure.Cosmos;
using System.Linq;
using BlazorCountries.Entities;
using BlazorCountries.Utility;

namespace BlazorCountries.Services
{
  public interface IPersonService
  {
    Person? Get(string id);
    Task<Person> AddAsync(Person item);
    Task<Person> UpdateAsync(Person item);
    Task DeleteAsync(Person item);
    Task Reset();
    PagedRes<Person> Search(PersonSearchReq dto);
  }

  public class PersonService : IPersonService
  {
    private Container _container;
    public PersonService(
        CosmosClient cosmosDbClient,
        string databaseName,
        string containerName)
    {
      _container = cosmosDbClient.GetContainer(databaseName, containerName);
    }
    public async Task<Person> AddAsync(Person item)
    {
      return await _container.CreateItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
    public async Task DeleteAsync(Person item)
    {
      var itemFind = Get(item.id);
      if (itemFind is null) return;

      await _container.DeleteItemAsync<Person>(item.id, new PartitionKey(Constants.PartitionKey));
    }
    public async Task Reset()
    {
      await _container.DeleteContainerAsync();
      await _container.Database.CreateContainerIfNotExistsAsync(Constants.PersonCt, "/userId");
    }
    public Person? Get(string id)
    {
      try
      {
        IQueryable<Person> queryable = _container.GetItemLinqQueryable<Person>(true);
        queryable = queryable.Where(item => item.id == id);
        return queryable.ToArray().FirstOrDefault();
      }
      catch (CosmosException) //For handling item not found and other exceptions
      {
        return null;
      }
    }
    public PagedRes<Person> Search(PersonSearchReq dto)
    {
      try
      {
        IQueryable<Person> queryable = _container.GetItemLinqQueryable<Person>(true);

        if (!string.IsNullOrEmpty(dto.FirstName))
        {
          queryable = queryable.Where(x => x.firstName.Contains(dto.FirstName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(dto.LastName))
        {
          queryable = queryable.Where(x => x.lastName.Contains(dto.LastName, StringComparison.OrdinalIgnoreCase));
        }

        // Count before paging
        var count = queryable.Count();

        // Sort
        if (string.IsNullOrEmpty(dto.OrderBy))
          queryable = queryable.OrderBy(x => x.firstName);
        else if (dto.OrderBy.Equals("name", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Ascending)
          queryable = queryable.OrderBy(x => x.firstName);
        else if (dto.OrderBy.Equals("name", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Descending)
          queryable = queryable.OrderByDescending(x => x.firstName);
        else
          queryable = queryable.OrderBy(x => x.firstName);

        // Get list after paging
        queryable = queryable
            .Skip(dto.Skip)
            .Take(dto.Take);
        var items = queryable.ToList();

        return new PagedRes<Person> { Items = items, Count = count };
      }
      catch (CosmosException) //For handling item not found and other exceptions
      {
        return new PagedRes<Person>();
      }
    }
    public async Task<Person> UpdateAsync(Person item)
    {
      var itemFind = Get(item.id);
      if (itemFind is null) throw new Exception("Cannot update.");

      return await _container.UpsertItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
  }
}
