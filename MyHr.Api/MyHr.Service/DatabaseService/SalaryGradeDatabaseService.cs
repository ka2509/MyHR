using Microsoft.Data.SqlClient;
using MyHr.Data.Model;
using MyHr.Service.Interface;

namespace MyHr.Service.DatabaseService
{
    internal class SalaryGradeDatabaseService : DatabaseService, ISalaryGradeDBService
    {
        public async Task<SalaryGrade?> GetSalaryGradeByIdAsync(string salaryGradeId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM SalaryGrade WHERE Id = @Id";
            cmd.Parameters.Add(new SqlParameter("@Id", salaryGradeId));
            using var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                return ReadSalaryGradeEntity(reader);
            }
            return null;
        }

        public async Task<SalaryGrade?> GetSalaryGradeByScaleAndLevel(string salaryScaleId, int gradeLevel)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT * FROM SalaryGrade 
                WHERE SalaryScaleId = @SalaryScaleId AND GradeLevel = @GradeLevel";
            cmd.Parameters.Add(new SqlParameter("@SalaryScaleId", salaryScaleId));
            cmd.Parameters.Add(new SqlParameter("@GradeLevel", gradeLevel));
            using var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                return ReadSalaryGradeEntity(reader);
            }
            return null;
        }

        public async Task<List<SalaryGrade>> GetSalaryGradesByScaleId(string salaryScaleId)
        {
            var result = new List<SalaryGrade>();
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT * FROM SalaryGrade 
                WHERE SalaryScaleId = @SalaryScaleId 
                ORDER BY GradeLevel";
            cmd.Parameters.Add(new SqlParameter("@SalaryScaleId", salaryScaleId));
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                result.Add(ReadSalaryGradeEntity(reader));
            }
            return result;
        }

        private SalaryGrade ReadSalaryGradeEntity(SqlDataReader reader)
        {
            return new SalaryGrade
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                SalaryScaleId = reader.GetString(reader.GetOrdinal("SalaryScaleId")),
                GradeLevel = reader.GetInt32(reader.GetOrdinal("GradeLevel")),
                SalaryCof = reader.GetDecimal(reader.GetOrdinal("SalaryCof")),
                BaseSalary = reader.GetDecimal(reader.GetOrdinal("BaseSalary")),
                PromotionMonthsMale = reader.GetInt32(reader.GetOrdinal("PromotionMonthsMale")),
                PromotionMonthsFemale = reader.GetInt32(reader.GetOrdinal("PromotionMonthsFemale"))
            };
        }
    }
}
