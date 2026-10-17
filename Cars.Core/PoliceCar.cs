 using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public class PoliceCar : Car, IRefuelable, IRepairable
{
    public bool SirenOn { get; private set; }

    public PoliceCar(
        string name,
        string model,
        int speed,
        int fuel,
        int condition,
        bool sirenOn)
        : base(name, model, speed, fuel, condition)
    {
        SirenOn = sirenOn;
    }

    public override string Drive()
    {
        return $"{Name} is patrolling Radiator Springs.";
    }

    public override ActionResult CrazyAction()
    {
        SirenOn = !SirenOn;

        string messageKey = SirenOn ? "PoliceSirenOn" : "PoliceSirenOff";

        NotifyCrazyAction();

        return new ActionResult(true, messageKey);

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

        return new ActionResult(true, "Refueled")   ;
    }

    public ActionResult Repair(int amount)
    {
        if (amount <= 0)
        {
            return new ActionResult(false, "InvalidRepairAmount");
        }

        if (!TryRepair(amount))
        {
            return new ActionResult(false, "RepairFailed");
        }

        return new ActionResult(true, "Repaired");
    }
}