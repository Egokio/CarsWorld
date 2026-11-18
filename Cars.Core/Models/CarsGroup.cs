using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public class CarGroup
{
    public Guid Id { get; }
    public string Name { get; private set; }

    private readonly List<Guid> carIds = new();

    public IReadOnlyCollection<Guid> CarIds => carIds.AsReadOnly();

    public CarGroup(string name, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name cannot be empty.");

        Id = id ?? Guid.NewGuid();
        Name = name;
    }

    public void AddCar(Guid carId)
    {
        if (carId == Guid.Empty)
            throw new ArgumentException("Car ID cannot be empty.");

        if (!carIds.Contains(carId))
            carIds.Add(carId);
    }

    public void RemoveCar(Guid carId)
    {
        carIds.Remove(carId);
    }
}