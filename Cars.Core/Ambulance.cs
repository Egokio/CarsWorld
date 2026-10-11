using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public class Ambulance : Car, IRefuelable
{
    public int PatientsTransported { get; private set; }

    public Ambulance(
        string name,
        string model,
        int speed,
        int fuel,
        int condition)
        : base(name, model, speed, fuel, condition)
    {
    }

    public override string Drive()
    {
        return $"{Name} is rushing through Radiator Springs with its siren wailing!";
    }

    public override ActionResult CrazyAction()
    {
        PatientsTransported++;

        return new ActionResult(true, "AmbulanceEmergencyRun", PatientsTransported);
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
