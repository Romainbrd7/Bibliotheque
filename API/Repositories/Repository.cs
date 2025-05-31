using System.Collections.Generic;
using System.Linq;
using API.Models;

namespace API.Repositories;

public class Repository<T> where T : Media
{
    private readonly List<T> _items = new();

    public IEnumerable<T> GetAll() => _items;

    public T? Get(int id) => _items.FirstOrDefault(item => item.Id == id);

    public void Add(T item) => _items.Add(item);

    public void Update(int id, T item)
    {
        var index = _items.FindIndex(i => i.Id == id);
        if (index != -1)
            _items[index] = item;
    }

    public void Delete(int id)
    {
        var item = Get(id);
        if (item != null)
            _items.Remove(item);
    }
}
