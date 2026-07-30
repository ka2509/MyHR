using Microsoft.Data.SqlClient;
using MyHr.Data.Model;
using MyHr.Service.Interface;

namespace MyHr.Service.DatabaseService
{
    internal class SalaryScaleDatabaseService : DatabaseService, ISalaryScaleDBService
    {
        public async Task<SalaryScale?> GetSalaryScaleByIdAsync(string salaryScaleId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM SalaryScale WHERE Id = @Id";
            cmd.Parameters.Add(new SqlParameter("@Id", salaryScaleId));
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadSalaryScaleEntity(reader);
            }
            return null;
        }

        public async Task<List<SalaryScale>> GetAllSalaryScalesAsync()
        {
            var result = new List<SalaryScale>();
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM SalaryScale ORDER BY Name";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(ReadSalaryScaleEntity(reader));
            }
            return result;
        }

        private SalaryScale ReadSalaryScaleEntity(SqlDataReader reader)
        {
            return new SalaryScale
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                MaxGrade = reader.GetInt32(reader.GetOrdinal("MaxGrade"))
            };
        }
    }
}
