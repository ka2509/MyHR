using Microsoft.Data.SqlClient;
using MyHr.Data.Enum;
using MyHr.Data.Model;
using MyHr.Service.Interface;

namespace MyHr.Service.DatabaseService
{
    internal class EmployeeDatabaseService : DatabaseService, IEmployeeDBService
    {
        public Task<bool> AddEmployees(List<Employee> employees)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            var result = new List<Employee>();
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Employee ORDER BY FullName";
            using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                result.Add(ReadEmployeeEntity(reader));
            }
            return result;
        }

        public async Task<Employee?> GetEmployeeByIdentityCardNumber(string identityCardNumber)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Employee WHERE IdentityCardNumber=@IdentityCardNumber";
            cmd.Parameters.Add(new SqlParameter("@IdentityCardNumber", identityCardNumber));
            using var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                var employee = ReadEmployeeEntity(reader);
                return employee;
            }
            return null;
        }

        public async Task<Employee?> GetEmployeeById(string employeeId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Employee WHERE Id=@Id";
            cmd.Parameters.Add(new SqlParameter("@Id", employeeId));
            using var reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                var employee = ReadEmployeeEntity(reader);
                return employee;
            }
            return null;
        }

        public async Task<bool> ImportEmployee(Employee employee)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Employee
                (
                    Id,
                    OrganizationId,
                    FullName,
                    Sex,
                    SocialInsurance,
                    Dob,
                    IdentityCardNumber,
                    SocialInsuranceContributionDate,
                    PositionId,
                    ProfessionId,
                    Password,
                    AllowanceId
                )
                VALUES
                (
                    @Id,
                    @OrganizationId,
                    @FullName,
                    @Sex,
                    @SocialInsurance,
                    @Dob,
                    @IdentityCardNumber,
                    @SocialInsuranceContributionDate,
                    @PositionId,
                    @ProfessionId,
                    @Password,
                    @AllowanceId
                );";
            cmd.Parameters.Add(new SqlParameter("@Id", employee.Id));
            cmd.Parameters.Add(new SqlParameter("@OrganizationId", employee.OrganizationId));
            cmd.Parameters.Add(new SqlParameter("@FullName", employee.FullName));
            cmd.Parameters.Add(new SqlParameter("@Sex", (Int32)employee.Sex));
            cmd.Parameters.Add(new SqlParameter("@SocialInsurance", employee.SocialInsurance));
            cmd.Parameters.Add(new SqlParameter("@Dob", employee.Dob));
            cmd.Parameters.Add(new SqlParameter("@IdentityCardNumber", employee.IdentityCardNumber));
            cmd.Parameters.Add(new SqlParameter("@SocialInsuranceContributionDate", employee.SocialInsuranceContributionDate));
            cmd.Parameters.Add(new SqlParameter("@PositionId", employee.PositionId));
            cmd.Parameters.Add(new SqlParameter("@ProfessionId", employee.ProfessionId));
            cmd.Parameters.Add(new SqlParameter("@Password", employee.Password));
            cmd.Parameters.Add(new SqlParameter("@AllowanceId", (object?)employee.AllowanceId ?? DBNull.Value));
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteEmployee(string employeeId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Employee WHERE Id=@Id";
            cmd.Parameters.Add(new SqlParameter("@Id", employeeId));
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        private Employee ReadEmployeeEntity(SqlDataReader reader)
        {
            var allowanceIdOrdinal = reader.GetOrdinal("AllowanceId");
            return new Employee
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                OrganizationId = reader.GetString(reader.GetOrdinal("OrganizationId")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                Sex = (Sex)reader.GetInt32(reader.GetOrdinal("Sex")),
                SocialInsurance = reader.GetString(reader.GetOrdinal("SocialInsurance")),
                Dob = reader.GetDateTime(reader.GetOrdinal("Dob")),
                IdentityCardNumber = reader.GetString(reader.GetOrdinal("IdentityCardNumber")),
                SocialInsuranceContributionDate = reader.GetDateTime(reader.GetOrdinal("SocialInsuranceContributionDate")),
                PositionId = reader.GetString(reader.GetOrdinal("PositionId")),
                ProfessionId = reader.GetString(reader.GetOrdinal("ProfessionId")),
                Password = reader.GetString(reader.GetOrdinal("Password")),
                AllowanceId = reader.IsDBNull(allowanceIdOrdinal) ? null : reader.GetString(allowanceIdOrdinal)
            };
        }
    }
}
