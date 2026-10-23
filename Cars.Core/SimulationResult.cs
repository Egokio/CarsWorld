using System;
using System.Collections.Generic;
using System.Text;


namespace Cars.Core;

/// <summary>
/// Stores the outcome and execution time of a car simulation.
/// </summary>

public sealed record SimulationResult(
    string CarName,
    string Status,
    long? Result,
    string? ErrorMessage,
    TimeSpan Duration);
