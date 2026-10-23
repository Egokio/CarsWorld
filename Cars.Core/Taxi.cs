namespace Cars.Core;

public class Taxi : Car, IRefuelable
{
    private const int WildRideDistanceKm = 10;

    public int FarePerKm { get; }
    public int RidesCompleted { get; private set; }
    public int Earnings { get; private set; }

    public Taxi(
        string name,
        string model,
        int speed,
        int fuel,
        int condition,
        int farePerKm)
        : base(name, model, speed, fuel, condition)
    {
        if (farePerKm <= 0)
        {
            throw new ArgumentException("Fare per km must be greater than 0.");
        }

        FarePerKm = farePerKm;
    }

    public override string Drive()
    {
        return $"{Name} is cruising around Radiator Springs looking for passengers.";
    }

    public override ActionResult CrazyAction()
    {
        RidesCompleted++;
        Earnings += FarePerKm * WildRideDistanceKm;

        NotifyCrazyAction();

        return new ActionResult(true, "TaxiWildRide", RidesCompleted, Earnings);
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
