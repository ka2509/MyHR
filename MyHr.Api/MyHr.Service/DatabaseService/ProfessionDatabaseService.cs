using Microsoft.Data.SqlClient;
using MyHr.Data.Model;
using MyHr.Service.Interface;
using System.Data;

namespace MyHr.Service.DatabaseService
{
    internal class ProfessionDatabaseService : DatabaseService, IProfessionDBService
    {
        public async Task<string?> GetProfessionNameByIdAsync(string professionId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT [Name] FROM Profession WHERE Id = @ProfessionId";
            cmd.Parameters.Add("@ProfessionId", SqlDbType.NVarChar, 50).Value = professionId;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetString(reader.GetOrdinal("Name"));
            }
            return null;
        }

        public async Task<Profession?> GetProfessionByIdAsync(string professionId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM Profession WHERE Id = @ProfessionId";
            cmd.Parameters.Add("@ProfessionId", SqlDbType.NVarChar, 50).Value = professionId;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var salaryScaleIdOrdinal = reader.GetOrdinal("SalaryScaleId");
                
                return new Profession
                {
                    Id = reader.GetString(reader.GetOrdinal("Id")),
                    Code = reader.GetString(reader.GetOrdinal("Code")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    SalaryScaleId = reader.IsDBNull(salaryScaleIdOrdinal) ? null : reader.GetString(salaryScaleIdOrdinal)
                };
            }
            return null;
        }

        public async Task<List<Profession>> GetAllProfessionsAsync()
        {
            var result = new List<Profession>();
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Profession ORDER BY Name";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var salaryScaleIdOrdinal = reader.GetOrdinal("SalaryScaleId");
                
                result.Add(new Profession
                {
                    Id = reader.GetString(reader.GetOrdinal("Id")),
                    Code = reader.GetString(reader.GetOrdinal("Code")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    SalaryScaleId = reader.IsDBNull(salaryScaleIdOrdinal) ? null : reader.GetString(salaryScaleIdOrdinal)
                });
            }
            return result;
        }
    }
}
