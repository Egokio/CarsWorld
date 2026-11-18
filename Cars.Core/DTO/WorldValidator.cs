using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public static class WorldValidator
{
    // Checks whether the loaded world contains valid and consistent data.
    public static void Validate(WorldDto world)
    {
        ArgumentNullException.ThrowIfNull(world);

        if (world.Cars is null || world.Groups is null)
            throw new InvalidDataException(
                "World data must contain cars and groups.");

        // Checks that every car has a unique ID.
        var carIds = new HashSet<Guid>();

        foreach (CarDto car in world.Cars)
        {
            if (car.Id == Guid.Empty)
                throw new InvalidDataException("A car has an empty ID.");

            if (!carIds.Add(car.Id))
                throw new InvalidDataException("Duplicate car ID found.");

            if (string.IsNullOrWhiteSpace(car.Name) ||
                string.IsNullOrWhiteSpace(car.Model))
            {
                throw new InvalidDataException(
                    "Every car must have a name and model.");
            }

            if (car.Speed < 0 ||
                car.Fuel < 0 || car.Fuel > 100 ||
                car.Condition < 0 || car.Condition > 100 ||
                car.ReactionCount < 0)
            {
                throw new InvalidDataException(
                    $"Car '{car.Name}' contains invalid values.");
            }

            // Checks whether the saved car type is supported.
            if (car.Type != nameof(RaceCar) &&
                car.Type != nameof(TowTruck) &&
                car.Type != nameof(PoliceCar))
            {
                throw new InvalidDataException(
                    $"Unknown car type: {car.Type}.");
            }
        }

        // Checks group IDs and their references to existing cars.
        var groupIds = new HashSet<Guid>();

        foreach (CarGroupDto group in world.Groups)
        {
            if (group.Id == Guid.Empty)
                throw new InvalidDataException("A group has an empty ID.");

            if (!groupIds.Add(group.Id))
                throw new InvalidDataException("Duplicate group ID found.");

            if (string.IsNullOrWhiteSpace(group.Name))
                throw new InvalidDataException(
                    "A group must have a name.");

            if (group.CarIds is null)
                throw new InvalidDataException(
                    $"Group '{group.Name}' has invalid car membership.");

            if (group.CarIds.Distinct().Count() != group.CarIds.Count)
                throw new InvalidDataException(
                    $"Group '{group.Name}' contains duplicate cars.");

            foreach (Guid carId in group.CarIds)
            {
                if (!carIds.Contains(carId))
                    throw new InvalidDataException(
                        $"Group '{group.Name}' references a car that does not exist.");
            }
        }
    }
}