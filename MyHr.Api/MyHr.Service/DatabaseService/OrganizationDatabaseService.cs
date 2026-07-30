using Microsoft.Data.SqlClient;
using MyHr.Data.Enum;
using MyHr.Data.Model;
using MyHr.Service.Interface;
using System.Data;

namespace MyHr.Service.DatabaseService
{
    internal class OrganizationDatabaseService : DatabaseService, IOrganizationDBService
    {
        public async Task<string?> GetOrganizationNameByIdAsync(String orgId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            SELECT [Name]
            FROM Organization
            WHERE Id = @OrgId";

            cmd.Parameters.Add("@OrgId", SqlDbType.NVarChar, 50).Value = orgId;

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetString(reader.GetOrdinal("Name"));
            }
            return null;
        }

        public async Task<List<Organization>> GetAllOrganizationsAsync()
        {
            var result = new List<Organization>();
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM Organization ORDER BY Type, Name";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(ReadOrganizationEntity(reader));
            }
            return result;
        }

        public async Task<Organization?> GetOrganizationByIdAsync(String orgId)
        {
            await using var conn = await GetSqlConnectionAsync();
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM Organization WHERE Id = @OrgId";
            cmd.Parameters.Add("@OrgId", SqlDbType.NVarChar, 50).Value = orgId;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadOrganizationEntity(reader);
            }
            return null;
        }

        private Organization ReadOrganizationEntity(SqlDataReader reader)
        {
            var parentIdOrdinal = reader.GetOrdinal("ParentId");
            return new Organization
            {
                Id = reader.GetString(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Type = (OrganizationType)reader.GetInt32(reader.GetOrdinal("Type")),
                ParentId = reader.IsDBNull(parentIdOrdinal) ? null : reader.GetString(parentIdOrdinal)
            };
        }
    }
}
