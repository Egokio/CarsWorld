using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public class WorldDto
{
    public List<CarDto> Cars { get; set; } = new();

    public List<CarGroupDto> Groups { get; set; } = new();
}