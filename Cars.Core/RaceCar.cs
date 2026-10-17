using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public class RaceCar : Car, IRefuelable
{
    public int TurboLevel { get; private set; }

    public RaceCar(
        string name,
        string model,
        int speed,
        int fuel,
        int condition,
        int turboLevel)
        : base(name, model, speed, fuel, condition)
    {
        if (turboLevel < 0 || turboLevel > 100)
        {
            throw new ArgumentException("Turbo level must be between 0 and 100.");
        }

        TurboLevel = turboLevel;
    }

    public override string Drive()
    {
        return $"{Name} is racing through Radiator Springs!";
    }

    public override ActionResult CrazyAction()
    {
        TurboLevel = Math.Min(100, TurboLevel + 20);

        NotifyCrazyAction();

        return new ActionResult(true, "RaceCarTurbo", TurboLevel);
    }

    public ActionResult Refuel(int amount)
    {
        if (amount <= 0)
        {
            return new ActionResult(false, "InvalidFuelAmount");
        }

        if (!TryAddFuel(amount))
        {
            return new ActionResult(false, "FuelFull");
        }

        return new ActionResult(true, "Refueled");
    }
}