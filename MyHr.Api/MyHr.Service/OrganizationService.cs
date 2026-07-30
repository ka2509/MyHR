using MyHr.Data.Dto;
using MyHr.Service.Interface;
using Microsoft.Extensions.Logging;

namespace MyHr.Service
{
    internal class OrganizationService : IOrganizationService
    {
        private readonly ILogger<OrganizationService> logger;
        private readonly IOrganizationDBService organizationDBService;

        public OrganizationService(ILogger<OrganizationService> logger, IOrganizationDBService organizationDBService)
        {
            this.logger = logger;
            this.organizationDBService = organizationDBService;
        }

        public async Task<string?> GetOrganizationNameById(string orgId)
        {
            var result = await organizationDBService.GetOrganizationNameByIdAsync(orgId);
            if (String.IsNullOrEmpty(result))
            {
                this.logger.LogWarning("GetOrganizationNameById failed: No organization found with ID {OrgId}", orgId);
            }
            return result;
        }

        public async Task<List<OrganizationResponse>> GetMainOrganizations()
        {
            var organizations = await organizationDBService.GetAllOrganizationsAsync();
            // TongCongTy (Type 0) and ChiNhanh (Type 1)
            return organizations
                .Where(o => o.Type == Data.Enum.OrganizationType.TongCongTy || 
                           o.Type == Data.Enum.OrganizationType.ChiNhanh)
                .Select(o => new OrganizationResponse
                {
                    Id = o.Id,
                    Name = o.Name,
                    Type = (int)o.Type,
                    ParentId = o.ParentId
                }).ToList();
        }

        public async Task<List<OrganizationResponse>> GetSubOrganizations(String parentId)
        {
            var organizations = await organizationDBService.GetAllOrganizationsAsync();
            // Phong (Type 2), Cum (Type 3), To (Type 4) - Filter to only these types
            return organizations
                .Where(o => o.ParentId == parentId && 
                           (o.Type == Data.Enum.OrganizationType.Phong || 
                            o.Type == Data.Enum.OrganizationType.Cum || 
                            o.Type == Data.Enum.OrganizationType.To))
                .Select(o => new OrganizationResponse
                {
                    Id = o.Id,
                    Name = o.Name,
                    Type = (int)o.Type,
                    ParentId = o.ParentId
                })
                .OrderBy(o => o.Type)
                .ThenBy(o => o.Name)
                .ToList();
        }
    }
}
