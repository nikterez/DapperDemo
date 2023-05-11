using WebAPI_Dapper.DTO;
using WebAPI_Dapper.Models;

namespace WebAPI_Dapper.Interfaces
{
    public interface ICompanyRepository
    {
        public Task<IEnumerable<Company>> GetAllCompanies();
        public Task<Company> GetCompanyById(int id);
        public Task<Company> CreateCompany(CreateCompanyDTO companyDTO);
        public Task CreateMultipleCompanies(IEnumerable<CreateCompanyDTO> companies);
        public Task UpdateCompany(int id, UpdateCompanyDTO company);
        public Task DeleteCompany(int id);
        public Task<Company> GetCompanyByEmployerId(int employerId);
        public Task<Company> GetMultipleResults(int id);
        public Task<IEnumerable<Company>> MultipleMapping();

    }
}
