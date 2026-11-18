using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;


namespace Cars.Core;

public class SqlWorldStorage : IWorldStorage
{
    private readonly DatabaseConnection databaseConnection;

    public SqlWorldStorage(DatabaseConnection databaseConnection)
    {
        this.databaseConnection = databaseConnection;
    }


    public async Task SaveAsync(
        WorldDto world,
        string path,
        CancellationToken token = default)
    {
        WorldValidator.Validate(world);

        await using SqlConnection connection =
            databaseConnection.CreateConnection();

        await connection.OpenAsync(token);

        using SqlTransaction transaction = connection.BeginTransaction();

        try
        {
            // Clear existing world data.
            await using (SqlCommand command = new SqlCommand(
                "DELETE FROM CarGroupMembers; DELETE FROM Cars; DELETE FROM CarGroups;",
                connection,
                transaction))
            {
                await command.ExecuteNonQueryAsync(token);
            }

            // Save cars.
            foreach (CarDto car in world.Cars)
            {
                await using SqlCommand command = new SqlCommand(
                    @"INSERT INTO Cars
                (Id, Type, Name, Model, Speed, Fuel, Condition,
                 ReactionCount, TurboLevel, TowCapacity, SirenOn)
                VALUES
                (@Id, @Type, @Name, @Model, @Speed, @Fuel, @Condition,
                 @ReactionCount, @TurboLevel, @TowCapacity, @SirenOn)",
                    connection,
                    transaction);

                command.Parameters.AddWithValue("@Id", car.Id);
                command.Parameters.AddWithValue("@Type", car.Type);
                command.Parameters.AddWithValue("@Name", car.Name);
                command.Parameters.AddWithValue("@Model", car.Model);
                command.Parameters.AddWithValue("@Speed", car.Speed);
                command.Parameters.AddWithValue("@Fuel", car.Fuel);
                command.Parameters.AddWithValue("@Condition", car.Condition);
                command.Parameters.AddWithValue("@ReactionCount", car.ReactionCount);

                command.Parameters.AddWithValue(
                    "@TurboLevel", (object?)car.TurboLevel ?? DBNull.Value);

                command.Parameters.AddWithValue(
                    "@TowCapacity", (object?)car.TowCapacity ?? DBNull.Value);

                command.Parameters.AddWithValue(
                    "@SirenOn", (object?)car.SirenOn ?? DBNull.Value);

                await command.ExecuteNonQueryAsync(token);
            }

            // Save groups.
            foreach (CarGroupDto group in world.Groups)
            {
                await using SqlCommand command = new SqlCommand(
                    "INSERT INTO CarGroups (Id, Name) VALUES (@Id, @Name)",
                    connection,
                    transaction);

                command.Parameters.AddWithValue("@Id", group.Id);
                command.Parameters.AddWithValue("@Name", group.Name);

                await command.ExecuteNonQueryAsync(token);
            }

            // Save car-group relationships.
            foreach (CarGroupDto group in world.Groups)
            {
                foreach (Guid carId in group.CarIds)
                {
                    await using SqlCommand command = new SqlCommand(
                        @"INSERT INTO CarGroupMembers (CarId, GroupId)
                      VALUES (@CarId, @GroupId)",
                        connection,
                        transaction);

                    command.Parameters.AddWithValue("@CarId", carId);
                    command.Parameters.AddWithValue("@GroupId", group.Id);

                    await command.ExecuteNonQueryAsync(token);
                }
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }



    public async Task<WorldDto> LoadAsync(
        string path,
        CancellationToken token = default)
    {
        await using SqlConnection connection =
            databaseConnection.CreateConnection();

        await connection.OpenAsync(token);

        List<CarDto> cars = new();
        List<CarGroupDto> groups = new();

        // Load cars.
        await using (SqlCommand command = new SqlCommand(
            @"SELECT Id, Type, Name, Model, Speed, Fuel, Condition,
                 ReactionCount, TurboLevel, TowCapacity, SirenOn
          FROM Cars",
            connection))
        await using (SqlDataReader reader =
            await command.ExecuteReaderAsync(token))
        {
            while (await reader.ReadAsync(token))
            {
                cars.Add(new CarDto
                {
                    Id = reader.GetGuid(0),
                    Type = reader.GetString(1),
                    Name = reader.GetString(2),
                    Model = reader.GetString(3),
                    Speed = reader.GetInt32(4),
                    Fuel = reader.GetInt32(5),
                    Condition = reader.GetInt32(6),
                    ReactionCount = reader.GetInt32(7),

                    TurboLevel = reader.IsDBNull(8)
                        ? null
                        : reader.GetInt32(8),

                    TowCapacity = reader.IsDBNull(9)
                        ? null
                        : reader.GetInt32(9),

                    SirenOn = reader.IsDBNull(10)
                        ? null
                        : reader.GetBoolean(10)
                });
            }
        }

        // Load groups.
        await using (SqlCommand command = new SqlCommand(
            "SELECT Id, Name FROM CarGroups",
            connection))
        await using (SqlDataReader reader =
            await command.ExecuteReaderAsync(token))
        {
            while (await reader.ReadAsync(token))
            {
                groups.Add(new CarGroupDto
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    CarIds = new List<Guid>()
                });
            }
        }

        // Load car-group relationships.
        await using (SqlCommand command = new SqlCommand(
            "SELECT CarId, GroupId FROM CarGroupMembers",
            connection))
        await using (SqlDataReader reader =
            await command.ExecuteReaderAsync(token))
        {
            while (await reader.ReadAsync(token))
            {
                Guid carId = reader.GetGuid(0);
                Guid groupId = reader.GetGuid(1);

                CarGroupDto? group = groups.FirstOrDefault(
                    g => g.Id == groupId);

                group?.CarIds.Add(carId);
            }
        }

        WorldDto world = new WorldDto
        {
            Cars = cars,
            Groups = groups
        };

        WorldValidator.Validate(world);

        return world;
    }

}

