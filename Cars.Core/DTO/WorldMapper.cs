using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;



public static class WorldMapper
{
    public static CarDto ToDto(Car car)
    {
        ArgumentNullException.ThrowIfNull(car);

        var dto = new CarDto
        {
            Id = car.Id,
            Name = car.Name,
            Model = car.Model,
            Speed = car.Speed,
            Fuel = car.Fuel,
            Condition = car.Condition,
            ReactionCount = car.ReactionCount
        };

        switch (car)
        {
            case RaceCar raceCar:
                dto.Type = nameof(RaceCar);
                dto.TurboLevel = raceCar.TurboLevel;
                break;

            case TowTruck towTruck:
                dto.Type = nameof(TowTruck);
                dto.TowCapacity = towTruck.TowCapacity;
                break;

            case PoliceCar policeCar:
                dto.Type = nameof(PoliceCar);
                dto.SirenOn = policeCar.SirenOn;
                break;

            default:
                throw new NotSupportedException(
                    $"Unsupported car type: {car.GetType().Name}");
        }

        return dto;
    }

    public static Car ToCar(CarDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.Id == Guid.Empty)
            throw new ArgumentException("Car ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.Model))
            throw new ArgumentException("Car name and model are required.");

        Car car = dto.Type switch
        {
            nameof(RaceCar) => new RaceCar(
                dto.Name,
                dto.Model,
                dto.Speed,
                dto.Fuel,
                dto.Condition,
                dto.TurboLevel
                    ?? throw new ArgumentException("Turbo level is missing."),
                dto.Id),

            nameof(TowTruck) => new TowTruck(
                dto.Name,
                dto.Model,
                dto.Speed,
                dto.Fuel,
                dto.Condition,
                dto.TowCapacity
                    ?? throw new ArgumentException("Tow capacity is missing."),
                dto.Id),

            nameof(PoliceCar) => new PoliceCar(
                dto.Name,
                dto.Model,
                dto.Speed,
                dto.Fuel,
                dto.Condition,
                dto.SirenOn
                    ?? throw new ArgumentException("Siren state is missing."),
                dto.Id),

            _ => throw new NotSupportedException(
                $"Unknown car type: {dto.Type}")
        };

        car.RestoreState(
            dto.Speed,
            dto.Fuel,
            dto.Condition,
            dto.ReactionCount);

        return car;
    }

    public static CarGroupDto ToDto(CarGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        return new CarGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            CarIds = group.CarIds.ToList()
        };
    }

    public static CarGroup ToGroup(CarGroupDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.Id == Guid.Empty)
            throw new ArgumentException("Group ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Group name cannot be empty.");

        var group = new CarGroup(dto.Name, dto.Id);

        foreach (Guid carId in dto.CarIds)
        {
            group.AddCar(carId);
        }

        return group;
    }

    public static WorldDto ToWorldDto(
    IEnumerable<Car> cars,
    IEnumerable<CarGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(cars);
        ArgumentNullException.ThrowIfNull(groups);

        return new WorldDto
        {
            Cars = cars
                .Select(ToDto)
                .ToList(),

            Groups = groups
                .Select(ToDto)
                .ToList()
        };
    }

    // Recreates all cars and groups from the saved world data.
    public static (List<Car> Cars, List<CarGroup> Groups) FromWorldDto(
        WorldDto world)
    {
        // Validates the complete world before creating any objects.
        WorldValidator.Validate(world);

        // Restores each car using its original type and ID.
        List<Car> cars = world.Cars
            .Select(ToCar)
            .ToList();

        // Restores groups and their saved car memberships.
        List<CarGroup> groups = world.Groups
            .Select(ToGroup)
            .ToList();

        return (cars, groups);
    }
}