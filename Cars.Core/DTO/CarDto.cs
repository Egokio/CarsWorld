using System;
using System.Collections.Generic;
using System.Text;

namespace Cars.Core;

public class CarDto
{
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

    public int Speed { get; set; }
    public int Fuel { get; set; }
    public int Condition { get; set; }

    public int ReactionCount { get; set; }

    // RaceCar
    public int? TurboLevel { get; set; }

    // TowTruck
    public int? TowCapacity { get; set; }

    // PoliceCar
    public bool? SirenOn { get; set; }
}