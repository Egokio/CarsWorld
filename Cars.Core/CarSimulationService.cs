using System.Diagnostics;

namespace Cars.Core;

/// <summary>
/// Performs CPU-intensive simulations using a snapshot of a car's data.
/// </summary>

public sealed class CarSimulationService
{
    public long Simulate(
        CarSnapshot car,
        int workload,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(car);


        if (workload <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(workload));
        }

        long score = 0;
        long iterations = (long)workload * 1_000_000;

        // Check for cancellation regularly during the CPU-intensive calculation.

        for (long i = 0; i < iterations; i++)
        {
            if (i % 10_000 == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            long speed = Math.Max(1, car.Speed);
            long fuel = Math.Max(1, car.Fuel);
            long condition = Math.Max(1, car.Condition);

            score += (i % speed + 1)
                   * (fuel + condition)
                   + (i % 97);
        }

        return score;
    }
}
