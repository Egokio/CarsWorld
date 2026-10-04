namespace Cars.Core;

public abstract class Car
{
    public string Name { get; }
    public string Model { get; }

    public int Speed { get; private set; }
    public int Fuel { get; private set; }
    public int Condition { get; private set; }

    protected Car(string name, string model, int speed, int fuel, int condition)
    {
        Name = name;
        Model = model;
        Speed = speed;
        Fuel = fuel;
        Condition = condition;
    }

    protected bool TryAddFuel(int amount)
    {
        if (amount <= 0 || Fuel >= 100)
        {
            return false;
        }

        Fuel = Math.Min(100, Fuel + amount);
        return true;
    }

    protected bool TryRepair(int amount)
    {
        if (amount <= 0 || Condition >= 100)
        {
            return false;
        }

        Condition = Math.Min(100, Condition + amount);
        return true;
    }

    public virtual string Drive()
    {
        return $"{Name} is driving through Radiator Springs.";
    }

    public abstract ActionResult CrazyAction();
}