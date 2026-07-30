using MyHr.Data.Dto;

namespace MyHr.Service.Interface
{
    public interface IOrganizationService
    {
        Task<String?> GetOrganizationNameById(String orgId);
        Task<List<OrganizationResponse>> GetMainOrganizations();
        Task<List<OrganizationResponse>> GetSubOrganizations(String parentId);
    }
}
