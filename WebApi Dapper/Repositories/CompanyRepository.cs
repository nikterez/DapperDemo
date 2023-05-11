using Dapper;
using System.Data;
using WebAPI_Dapper.Data;
using WebAPI_Dapper.DTO;
using WebAPI_Dapper.Interfaces;
using WebAPI_Dapper.Models;

namespace WebAPI_Dapper.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;
        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Company> CreateCompany(CreateCompanyDTO companyDTO)
        {
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)" +
                        "SELECT CAST(SCOPE_IDENTITY() AS int)";

            var parameters = new DynamicParameters();
            parameters.Add("Name", companyDTO.Name, DbType.String);
            parameters.Add("Address", companyDTO.Address, DbType.String);
            parameters.Add("Country", companyDTO.Country, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);

                var newCompany = new Company
                {
                    Id = id,
                    Name = companyDTO.Name,
                    Address = companyDTO.Address,
                    Country = companyDTO.Country
                };

                return newCompany;
            }
        }

        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            var query = "SELECT Id, Name, Address, Country FROM Companies";

            using (var connection = _context.CreateConnection())
            {
                var companies = await connection.QueryAsync<Company>(query);
                return companies.ToList();
            }

        }

        public async Task<Company> GetCompanyById(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });
                return company;
            }
        }

        public async Task UpdateCompany(int id, UpdateCompanyDTO company)
        {
            var query = "UPDATE Companies SET Name = @Name, Address = @Address, Country = @Country" +
                        "WHERE Id = @Id";

            var parameters = new DynamicParameters();

            parameters.Add("id", id, DbType.Int32);
            parameters.Add("name", company.Name, DbType.String);
            parameters.Add("address", company.Address, DbType.String);
            parameters.Add("country", company.Country, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
        public async Task DeleteCompany(int id)
        {
            var query = "DELETE FROM Companies WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task<Company> GetCompanyByEmployerId(int employerId)
        {
            var spName = "ShowCompanyByEmployeeId";
            var parameters = new DynamicParameters();
            parameters.Add("Id", employerId, DbType.Int32, ParameterDirection.Input);

            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QueryFirstOrDefaultAsync<Company>(spName, parameters, commandType: CommandType.StoredProcedure);

                return company;
            }
        }

        public async Task<Company> GetMultipleResults(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id = @Id;" +
                        "SELECT * FROM Employees WHERE CompanyId = @Id";

            using (var connection = _context.CreateConnection())
            {
                using (var multi = await connection.QueryMultipleAsync(query, new { Id = id }))
                {
                    var company = await multi.ReadSingleOrDefaultAsync<Company>();
                    if (company != null)
                    {
                        company.Employees = (await multi.ReadAsync<Employee>()).ToList();
                    }

                    return company;
                }
            }
        }

        public async Task<IEnumerable<Company>> MultipleMapping()
        {
            var query = "SELECT * FROM Companies c JOIN Employees e ON c.Id = e.CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var companyDict = new Dictionary<int, Company>();

                var companies = await connection.QueryAsync<Company, Employee, Company>(
                    query, (company, employee) =>
                    {
                        if (!companyDict.TryGetValue(company.Id, out var curCompany))
                        {
                            curCompany = company;
                            companyDict.Add(curCompany.Id, curCompany);
                        }

                        curCompany.Employees.Add(employee);

                        return curCompany;
                    });

                return companies.Distinct().ToList();
            }
        }

        public async Task CreateMultipleCompanies(IEnumerable<CreateCompanyDTO> companies)
        {
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var c in companies)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", c.Name, DbType.String);
                        parameters.Add("Address", c.Address, DbType.String);
                        parameters.Add("Country", c.Country, DbType.String);

                        await connection.ExecuteAsync(query, parameters, transaction: transaction);
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
