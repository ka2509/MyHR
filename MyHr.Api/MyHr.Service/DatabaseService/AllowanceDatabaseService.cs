using Microsoft.Data.SqlClient;
using MyHr.Data.Enum;
using MyHr.Data.Model;
using MyHr.Service.Interface;

namespace MyHr.Service.DatabaseService
{
    internal class AllowanceDatabaseService : DatabaseService, IAllowanceDBService
    {
        public async Task<Allowance?> GetAllowanceByIdAsync(string allowanceId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Allowance WHERE Id = @Id";
            cmd.Parameters.Add(new SqlParameter("@Id", allowanceId));
            
            using var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                return ReadAllowanceEntity(reader);
            }
            return null;
        }

        public async Task<List<Allowance>> GetAllAllowancesAsync()
        {
            var result = new List<Allowance>();
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Allowance ORDER BY Type, Level";
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                result.Add(ReadAllowanceEntity(reader));
            }
            return result;
        }

        private Allowance ReadAllowanceEntity(SqlDataReader reader)
        {
            return new Allowance
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                Type = (AllowanceType)reader.GetInt32(reader.GetOrdinal("Type")),
                Level = reader.GetInt32(reader.GetOrdinal("Level")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Coefficient = reader.GetDecimal(reader.GetOrdinal("Coefficient"))
            };
        }
    }
}
