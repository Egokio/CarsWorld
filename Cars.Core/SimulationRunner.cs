using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

namespace Cars.Core;

/// <summary>
/// Runs car simulations sequentially or in parallel.
/// </summary>
public sealed class SimulationRunner
{
    private readonly CarSimulationService simulationService = new();

    public async Task<SimulationReport> RunSequentialAsync(
        IReadOnlyList<CarSnapshot> cars,
        int workload,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var results = new List<SimulationResult>();

        foreach (var car in cars)
        {
            results.Add(await Task.Run(
                () => RunOne(car, workload, cancellationToken)));
        }

        stopwatch.Stop();
        return new SimulationReport(results, stopwatch.Elapsed);
    }

    public async Task<SimulationReport> RunParallelAsync(
        IReadOnlyList<CarSnapshot> cars,
        int workload,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var tasks = cars.Select(car => Task.Run(
            () => RunOne(car, workload, cancellationToken))).ToArray();

        var results = await Task.WhenAll(tasks);

        stopwatch.Stop();
        return new SimulationReport(results, stopwatch.Elapsed);
    }

    private SimulationResult RunOne(
        CarSnapshot car,
        int workload,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            long result = simulationService.Simulate(
                car, workload, cancellationToken);

            return new SimulationResult(
                car.Name, "Success", result, null, stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            return new SimulationResult(
                car.Name, "Cancelled", null,
                "The simulation was cancelled.", stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            return new SimulationResult(
                car.Name, "Error", null, ex.Message, stopwatch.Elapsed);
        }
    }
}
