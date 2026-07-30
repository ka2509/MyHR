using Microsoft.Data.SqlClient;
using MyHr.Data.Model;
using MyHr.Service.Interface;
using System.Data;

namespace MyHr.Service.DatabaseService
{
    internal class PositionDatabaseService : DatabaseService, IPositionDBService
    {
        public async Task<string?> GetPositionNameByIdAsync(string positionId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT [Name] FROM Position WHERE Id = @PositionId";
            cmd.Parameters.Add("@PositionId", SqlDbType.NVarChar, 50).Value = positionId;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetString(reader.GetOrdinal("Name"));
            }
            return null;
        }

        public async Task<Position?> GetPositionByIdAsync(string positionId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Position WHERE Id = @PositionId";
            cmd.Parameters.Add("@PositionId", SqlDbType.NVarChar, 50).Value = positionId;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadPositionEntity(reader);
            }
            return null;
        }

        public async Task<List<Position>> GetAllPositionsAsync()
        {
            var result = new List<Position>();
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Position ORDER BY Name";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(ReadPositionEntity(reader));
            }
            return result;
        }

        private Position ReadPositionEntity(SqlDataReader reader)
        {
            var salaryScaleIdOrdinal = reader.GetOrdinal("SalaryScaleId");
            
            return new Position
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                AllowanceCof = reader.GetDecimal(reader.GetOrdinal("AllowanceCof")),
                IsManagement = reader.GetBoolean(reader.GetOrdinal("IsManagement")),
                SalaryScaleId = reader.IsDBNull(salaryScaleIdOrdinal) ? null : reader.GetString(salaryScaleIdOrdinal)
            };
        }
    }
}
