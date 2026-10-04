using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public interface IRefuelable
{
    ActionResult Refuel(int amount);
}
