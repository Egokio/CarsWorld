using System;
using System.Collections.Generic;
using System.Text;


namespace Cars.Core;

/// <summary>
/// Contains the results and total execution time of a simulation run.
/// </summary>

public sealed record SimulationReport(
    IReadOnlyList<SimulationResult> Results,
    TimeSpan TotalDuration);
