using System;
using System.Collections.Generic;
using System.Text;


namespace Cars.Core;

public class CarEventArgs : EventArgs
{
    public Car OtherCar { get; }

    public CarEventArgs(Car otherCar)
    {
        OtherCar = otherCar;
    }
}
