using System.Data;
using WebApi.Models;
using WebApi.DTOs.Filters;
using Oracle.ManagedDataAccess.Client;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories
{
    public class OraclePositionRepository : IPositionRepository
    {
        private readonly string _connectionString;

        public OraclePositionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }
        public async Task<IEnumerable<Position>> GetAllAsync()
        {
            var list = new List<Position>();
            await using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID, TITLE, DESCRIPTION, LOCATION, STATUS, RECRUITER_ID, DEPARTMENT_ID, BUDGET, CLOSING_DATE
                                FROM POSITIONS";
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Position
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    Title = reader.GetString(reader.GetOrdinal("TITLE")),
                    Description = reader.GetString(reader.GetOrdinal("DESCRIPTION")),
                    Location = reader.GetString(reader.GetOrdinal("LOCATION")),
                    Status = reader.GetString(reader.GetOrdinal("STATUS")),
                    RecruiterId = reader.GetInt32(reader.GetOrdinal("RECRUITER_ID")),
                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DEPARTMENT_ID")),
                    Budget = reader.GetDecimal(reader.GetOrdinal("BUDGET")),
                    ClosingDate = reader.IsDBNull(reader.GetOrdinal("CLOSING_DATE"))
                                   ? null
                                   : reader.GetDateTime(reader.GetOrdinal("CLOSING_DATE"))
                });
            }
            return list;
        }
        public async Task<Position?> GetByIdAsync(int id)
        {
            await using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID, TITLE, DESCRIPTION, LOCATION, STATUS, RECRUITER_ID, DEPARTMENT_ID, BUDGET, CLOSING_DATE
                                FROM POSITIONS
                                WHERE ID = :id";
            cmd.Parameters.Add(new OracleParameter("id", id));
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;
            return new Position
            {
                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                Title = reader.GetString(reader.GetOrdinal("TITLE")),
                Description = reader.GetString(reader.GetOrdinal("DESCRIPTION")),
                Location = reader.GetString(reader.GetOrdinal("LOCATION")),
                Status = reader.GetString(reader.GetOrdinal("STATUS")),
                RecruiterId = reader.GetInt32(reader.GetOrdinal("RECRUITER_ID")),
                DepartmentId = reader.GetInt32(reader.GetOrdinal("DEPARTMENT_ID")),
                Budget = reader.GetDecimal(reader.GetOrdinal("BUDGET")),
                ClosingDate = reader.IsDBNull(reader.GetOrdinal("CLOSING_DATE"))
                              ? null
                              : reader.GetDateTime(reader.GetOrdinal("CLOSING_DATE"))
            };
        }
        public async Task<Position> CreateAsync(Position position)
        {
            await using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.BindByName = true;
            cmd.CommandText = @"INSERT INTO POSITIONS (TITLE, DESCRIPTION, LOCATION, STATUS, RECRUITER_ID, DEPARTMENT_ID, BUDGET, CLOSING_DATE)
                                VALUES (:p_title, :p_description, :p_location, :p_status, :p_recruiterId, :p_departmentId, :p_budget, :p_closingDate)
                                RETURNING ID INTO :p_newId";
            cmd.Parameters.Add(new OracleParameter("p_title", position.Title));
            cmd.Parameters.Add(new OracleParameter("p_description", position.Description));
            cmd.Parameters.Add(new OracleParameter("p_location", position.Location));
            cmd.Parameters.Add(new OracleParameter("p_status", position.Status));
            cmd.Parameters.Add(new OracleParameter("p_recruiterId", position.RecruiterId));
            cmd.Parameters.Add(new OracleParameter("p_departmentId", position.DepartmentId));
            cmd.Parameters.Add(new OracleParameter("p_budget", position.Budget));
            cmd.Parameters.Add(new OracleParameter("p_closingDate", position.ClosingDate ?? (object)DBNull.Value));
            var outParam = new OracleParameter
            {
                ParameterName = "p_newId",
                OracleDbType = OracleDbType.Int32,
                Direction = ParameterDirection.Output,
                Size = sizeof(int)
            };
            cmd.Parameters.Add(outParam);
            await cmd.ExecuteNonQueryAsync();
            var newIdString = outParam.Value.ToString();
            position.Id = int.Parse(newIdString);
            return position;
        }
        public async Task<bool> UpdateAsync(Position position)
        {
            await using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.BindByName = true;
            cmd.CommandText = @"UPDATE POSITIONS
                                SET TITLE = :p_title,
                                    DESCRIPTION = :p_description,
                                    LOCATION = :p_location,
                                    STATUS = :p_status,
                                    RECRUITER_ID = :p_recruiterId,
                                    DEPARTMENT_ID = :p_departmentId,
                                    BUDGET = :p_budget,
                                    CLOSING_DATE = :p_closingDate
                                 WHERE ID = :p_id";
            cmd.Parameters.Add(new OracleParameter("p_title", position.Title));
            cmd.Parameters.Add(new OracleParameter("p_description", position.Description));
            cmd.Parameters.Add(new OracleParameter("p_location", position.Location));
            cmd.Parameters.Add(new OracleParameter("p_status", position.Status));
            cmd.Parameters.Add(new OracleParameter("p_recruiterId", position.RecruiterId));
            cmd.Parameters.Add(new OracleParameter("p_departmentId", position.DepartmentId));
            cmd.Parameters.Add(new OracleParameter("p_budget", position.Budget));
            cmd.Parameters.Add(new OracleParameter("p_closingDate", position.ClosingDate ?? (object)DBNull.Value));
            cmd.Parameters.Add(new OracleParameter("p_id", position.Id));
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            await using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM POSITIONS WHERE ID = :id";
            cmd.Parameters.Add(new OracleParameter("id", id));
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        public async Task<IEnumerable<Position>> GetFilteredAsync(PositionFilter filter)
        {
            var list = new List<Position>();

            await using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"SELECT  ID, TITLE, DESCRIPTION, LOCATION, STATUS, RECRUITER_ID, DEPARTMENT_ID, BUDGET, CLOSING_DATE
                         FROM   POSITIONS
                         WHERE  (:p_status    IS NULL OR STATUS   = :p_status)
                                AND (:p_location  IS NULL OR LOCATION = :p_location)
                         ORDER BY ID
                         OFFSET :p_offset ROWS FETCH NEXT :p_pageSize ROWS ONLY";
            await using var cmd = conn.CreateCommand();
            cmd.BindByName = true;
            cmd.CommandText = sql;
            cmd.Parameters.Add(new OracleParameter("p_status", filter.Status ?? (object)DBNull.Value));
            cmd.Parameters.Add(new OracleParameter("p_location", filter.Location ?? (object)DBNull.Value));
            var offset = (filter.Page - 1) * filter.PageSize;
            cmd.Parameters.Add(new OracleParameter("p_offset", offset));
            cmd.Parameters.Add(new OracleParameter("p_pageSize", filter.PageSize));
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Position
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    Title = reader.GetString(reader.GetOrdinal("TITLE")),
                    Description = reader.GetString(reader.GetOrdinal("DESCRIPTION")),
                    Location = reader.GetString(reader.GetOrdinal("LOCATION")),
                    Status = reader.GetString(reader.GetOrdinal("STATUS")),
                    RecruiterId = reader.GetInt32(reader.GetOrdinal("RECRUITER_ID")),
                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DEPARTMENT_ID")),
                    Budget = reader.GetDecimal(reader.GetOrdinal("BUDGET")),
                    ClosingDate = reader.IsDBNull(reader.GetOrdinal("CLOSING_DATE"))
                                  ? null
                                  : reader.GetDateTime(reader.GetOrdinal("CLOSING_DATE"))
                });
            }

            return list;
        }
    }
}
