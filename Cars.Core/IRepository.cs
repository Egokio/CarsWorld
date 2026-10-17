using System;
using System.Collections.Generic;
using System.Text;


namespace Cars.Core;

public interface IRepository<T> where T : Car
{
    void Add(T item);

    bool Remove(T item);

    IEnumerable<T> GetAll();

    T? Find(Func<T, bool> predicate);
}
