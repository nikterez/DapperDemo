using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using WebAPI_Dapper.DTO;
using WebAPI_Dapper.Interfaces;
using WebAPI_Dapper.Mapping;

namespace WebAPI_Dapper.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly CompanyMapper _mapper;

        public CompaniesController(ICompanyRepository companyRepo, CompanyMapper mapper)
        {
            _companyRepo = companyRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = (await _companyRepo.GetAllCompanies()).ToList();
            //List<CreateCompanyDTO> z = companies.Select(c=>_mapper.CoToCCDTO(c)).ToList();
            return Ok(companies);
        }

        [HttpGet("{id}", Name = "GetCompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                var company = await _companyRepo.GetCompanyById(id);
                return Ok(company);
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDTO companyDTO)
        {
            var createdCompany = await _companyRepo.CreateCompany(companyDTO);

            return CreatedAtRoute("GetCompanybyId", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyDTO companyDTO)
        {
            var company = await _companyRepo.GetCompanyById(id);

            if (company == null)
            {
                return NotFound();
            }

            await _companyRepo.UpdateCompany(id, companyDTO);

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _companyRepo.GetCompanyById(id);

            if (company == null)
            {
                return NotFound();
            }

            await _companyRepo.DeleteCompany(id);

            return NoContent();
        }

        [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> GetCompanyForEmployee(int id)
        {
            var company = await _companyRepo.GetCompanyByEmployerId(id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetMultipleResults(int id) 
        {
            var company = await _companyRepo.GetMultipleResults(id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        [HttpGet("MultipleMapping")]
        public async Task<IActionResult> MultipleMapping()
        {
            var companies = await _companyRepo.MultipleMapping();

            return Ok(companies);
        }

    }
}
