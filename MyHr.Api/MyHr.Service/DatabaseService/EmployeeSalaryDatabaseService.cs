using Microsoft.Data.SqlClient;
using MyHr.Data.Model;
using MyHr.Service.Interface;

namespace MyHr.Service.DatabaseService
{
    internal class EmployeeSalaryDatabaseService : DatabaseService, IEmployeeSalaryDBService
    {
        public async Task<bool> AddEmployeeSalary(EmployeeSalary employeeSalary)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO EmployeeSalary
                (
                    Id,
                    EmployeeId,
                    SalaryGradeId,
                    EffectiveFrom,
                    EffectiveTo,
                    Reason,
                    FixedSalaryAmount,
                    TotalSalary
                )
                VALUES
                (
                    @Id,
                    @EmployeeId,
                    @SalaryGradeId,
                    @EffectiveFrom,
                    @EffectiveTo,
                    @Reason,
                    @FixedSalaryAmount,
                    @TotalSalary
                );";
            cmd.Parameters.Add(new SqlParameter("@Id", employeeSalary.Id));
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeSalary.EmployeeId));
            cmd.Parameters.Add(new SqlParameter("@SalaryGradeId", (object?)employeeSalary.SalaryGradeId ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@EffectiveFrom", employeeSalary.EffectiveFrom));
            cmd.Parameters.Add(new SqlParameter("@EffectiveTo", (object?)employeeSalary.EffectiveTo ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Reason", (object?)employeeSalary.Reason ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@FixedSalaryAmount", (object?)employeeSalary.FixedSalaryAmount ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@TotalSalary", employeeSalary.TotalSalary));
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<EmployeeSalary?> GetCurrentSalaryByEmployeeId(string employeeId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT * FROM EmployeeSalary 
                WHERE EmployeeId = @EmployeeId AND EffectiveTo IS NULL";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            using var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                return ReadEmployeeSalaryEntity(reader);
            }
            return null;
        }

        public async Task<List<EmployeeSalary>> GetSalaryHistoryByEmployeeId(string employeeId)
        {
            var result = new List<EmployeeSalary>();
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT * FROM EmployeeSalary 
                WHERE EmployeeId = @EmployeeId 
                ORDER BY EffectiveFrom DESC";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                result.Add(ReadEmployeeSalaryEntity(reader));
            }
            return result;
        }

        public async Task<bool> DeleteEmployeeSalaries(string employeeId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM EmployeeSalary WHERE EmployeeId=@EmployeeId";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            await cmd.ExecuteNonQueryAsync();
            return true; // Returns true even if no rows deleted (employee might not have salary records)
        }

        private EmployeeSalary ReadEmployeeSalaryEntity(SqlDataReader reader)
        {
            var salaryGradeIdOrdinal = reader.GetOrdinal("SalaryGradeId");
            var effectiveToOrdinal = reader.GetOrdinal("EffectiveTo");
            var reasonOrdinal = reader.GetOrdinal("Reason");
            var fixedSalaryAmountOrdinal = reader.GetOrdinal("FixedSalaryAmount");
            
            return new EmployeeSalary
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                EmployeeId = reader.GetString(reader.GetOrdinal("EmployeeId")),
                SalaryGradeId = reader.IsDBNull(salaryGradeIdOrdinal) ? null : reader.GetString(salaryGradeIdOrdinal),
                EffectiveFrom = reader.GetDateTime(reader.GetOrdinal("EffectiveFrom")),
                EffectiveTo = reader.IsDBNull(effectiveToOrdinal) ? null : reader.GetDateTime(effectiveToOrdinal),
                Reason = reader.IsDBNull(reasonOrdinal) ? null : reader.GetString(reasonOrdinal),
                FixedSalaryAmount = reader.IsDBNull(fixedSalaryAmountOrdinal) ? null : reader.GetDecimal(fixedSalaryAmountOrdinal),
                TotalSalary = reader.GetDecimal(reader.GetOrdinal("TotalSalary"))
            };
        }
    }
}
