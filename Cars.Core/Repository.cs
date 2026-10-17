using System;
using System.Collections.Generic;
using System.Text;



namespace Cars.Core;

public class Repository<T> : IRepository<T> where T : Car
{
    private readonly List<T> items = new();

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        foreach (T existing in items)
        {
            existing.NotifyGreeted(item);
            existing.ReactTo(item);
        }

        items.Add(item);
        Subscribe(item);
    }

    public bool Remove(T item)
    {
        if (!items.Remove(item))
            return false;

        Unsubscribe(item);

        foreach (T existing in items)
        {
            existing.NotifyFarewell(item);
            existing.ReactTo(item);
        }

        return true;
    }

    public IEnumerable<T> GetAll()
    {
        return items.ToList();
    }

    public T? Find(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return items.FirstOrDefault(predicate);
    }

    private void Subscribe(T item)
    {
        item.CrazyActionPerformed += OnCrazyActionPerformed;
    }

    private void Unsubscribe(T item)
    {
        item.CrazyActionPerformed -= OnCrazyActionPerformed;
    }

    private void OnCrazyActionPerformed(object? sender, CarEventArgs e)
    {
        foreach (T other in items)
        {
            if (!ReferenceEquals(other, sender))
            {
                other.ReactTo((Car)sender!);
            }
        }
    }
}
