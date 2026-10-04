using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public static class CarValidator
{
    public static bool IsValidName(string name)
    {
        return !string.IsNullOrWhiteSpace(name);
    }

    public static bool IsValidSpeed(int speed)
    {
        return speed >= 0 && speed <= 300;
    }

    public static bool IsValidFuel(int fuel)
    {
        return fuel >= 0 && fuel <= 100;
    }

    public static bool IsValidCondition(int condition)
    {
        return condition >= 0 && condition <= 100;
    }
}