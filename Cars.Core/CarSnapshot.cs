using System;
using System.Collections.Generic;
using System.Text;


namespace Cars.Core;

public sealed record CarSnapshot(
    string Name,
    string Model,
    int Speed,
    int Fuel,
    int Condition)
{
    public static CarSnapshot FromCar(Car car)
    {
        ArgumentNullException.ThrowIfNull(car);

        return new CarSnapshot(
            car.Name,
            car.Model,
            car.Speed,
            car.Fuel,
            car.Condition);
    }
}
