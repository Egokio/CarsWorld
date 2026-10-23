namespace Cars.Core;

public class FireTruck : Car, IRefuelable, IRepairable
{
    private const int WaterPerSpray = 25;

    public int WaterLevel { get; private set; }

    public FireTruck(
        string name,
        string model,
        int speed,
        int fuel,
        int condition,
        int waterLevel)
        : base(name, model, speed, fuel, condition)
    {
        if (waterLevel < 0 || waterLevel > 100)
        {
            throw new ArgumentException("Water level must be between 0 and 100.");
        }

        WaterLevel = waterLevel;
    }

    public override string Drive()
    {
        return $"{Name} is racing to the fire with its horn blaring!";
    }

    public override ActionResult CrazyAction()
    {
        string messageKey;

        if (WaterLevel < WaterPerSpray)
        {
            // Not enough water left: refill the tank at the hydrant.
            WaterLevel = 100;
            messageKey = "FireTruckRefill";
        }
        else
        {
            WaterLevel -= WaterPerSpray;
            messageKey = "FireTruckSpray";
        }

        NotifyCrazyAction();

        return new ActionResult(true, messageKey, WaterLevel);
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
