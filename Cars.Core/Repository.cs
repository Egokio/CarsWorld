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

    // Restores an existing car without triggering greeting or reaction events.
    public void Restore(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Prevents adding the same car ID more than once.
        if (items.Any(existing => existing.Id == item.Id))
            throw new InvalidOperationException(
                "A car with this ID already exists in the repository.");

        // Adds the restored car and reconnects its event subscription.
        items.Add(item);
        Subscribe(item);
    }

    // Removes all cars and disconnects their event subscriptions.
    public void Clear()
    {
        // Unsubscribes every car before removing it.
        foreach (T item in items)
        {
            Unsubscribe(item);
        }

        items.Clear();
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
