using Microsoft.Extensions.Logging;
using MyHr.Service.Interface;

namespace MyHr.Service
{
    internal class ProfessionService : IProfessionService
    {
        private readonly IProfessionDBService professionDBService;
        private readonly ILogger<ProfessionService> logger;
        public ProfessionService(IProfessionDBService professionDBService, ILogger<ProfessionService> logger)
        {
            this.professionDBService = professionDBService;
            this.logger = logger;
        }
        public async Task<string?> GetProfessionNameById(string professionId)
        {
            var result = await professionDBService.GetProfessionNameByIdAsync(professionId);
            if (String.IsNullOrEmpty(result))
            {
                this.logger.LogWarning("GetProfessionNameById failed: No organization found with ID {OrgId}", professionId);
            }
            return result;
        }
    }
}
