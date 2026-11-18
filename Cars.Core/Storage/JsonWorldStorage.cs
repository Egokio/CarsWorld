using System;
using System.Collections.Generic;
using System.Text;

using System.Text.Json;

namespace Cars.Core;

public class JsonWorldStorage : IWorldStorage
{
    private readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };

    public async Task SaveAsync(
        WorldDto world,
        string path,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(world);

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path cannot be empty.");

        await using FileStream stream = new(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        await JsonSerializer.SerializeAsync(
            stream,
            world,
            options,
            token);
    }

    public async Task<WorldDto> LoadAsync(
        string path,
        CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path cannot be empty.");

        if (!File.Exists(path))
            throw new FileNotFoundException(
                "The save file was not found.",
                path);

        await using FileStream stream = new(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        // Reads the saved world data from the JSON file.
        WorldDto? world = await JsonSerializer.DeserializeAsync<WorldDto>(
            stream,
            options,
            token);

        // Rejects an empty or invalid world.
        if (world is null)
            throw new InvalidDataException(
                "The JSON file does not contain valid world data.");

        // Validates all cars, groups and their relationships.
        WorldValidator.Validate(world);

        return world;
    }
}