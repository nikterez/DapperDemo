using Riok.Mapperly.Abstractions;
using WebAPI_Dapper.DTO;
using WebAPI_Dapper.Models;

namespace WebAPI_Dapper.Mapping
{
    [Mapper]
    public partial class CompanyMapper
    {
        public partial CreateCompanyDTO CoToCCDTO(Company company);
        public partial Company CoCCDTOToCo(CreateCompanyDTO company);

    }
}
