using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public class TowTruck : Car, IRepairable
{
    public int TowCapacity { get; private set; }

    public TowTruck(
        string name,
        string model,
        int speed,
        int fuel,
        int condition,
        int towCapacity)
        : base(name, model, speed, fuel, condition)
    {
        if (towCapacity <= 0)
        {
            throw new ArgumentException("Tow capacity must be greater than 0.");
        }

        TowCapacity = towCapacity;
    }

    public override string Drive()
    {
        return $"{Name} is slowly driving around Radiator Springs.";
    }

    public override ActionResult CrazyAction()
    {

        NotifyCrazyAction();

        return new ActionResult(true, "TowTruckRescue");
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