using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Cars.Core;

public class XmlWorldStorage : IWorldStorage
{
    private readonly XmlSerializer serializer =
        new(typeof(WorldDto));

    public async Task SaveAsync(
        WorldDto world,
        string path,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(world);

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path cannot be empty.");

        using var memoryStream = new MemoryStream();

        serializer.Serialize(memoryStream, world);

        await File.WriteAllBytesAsync(
            path,
            memoryStream.ToArray(),
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

        byte[] data = await File.ReadAllBytesAsync(path, token);

        using var memoryStream = new MemoryStream(data);

        // Reads the saved world data from the XML file.
        WorldDto? world = serializer.Deserialize(memoryStream) as WorldDto;

        // Rejects an empty or invalid world.
        if (world is null)
            throw new InvalidDataException(
                "The XML file does not contain valid world data.");

        // Validates all cars, groups and their relationships.
        WorldValidator.Validate(world);

        return world;
    }
}