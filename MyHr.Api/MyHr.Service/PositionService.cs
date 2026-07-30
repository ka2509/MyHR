using Microsoft.Extensions.Logging;
using MyHr.Service.Interface;

namespace MyHr.Service
{
    internal class PositionService : IPositionService
    {
        private readonly ILogger<PositionService> logger;
        private readonly IPositionDBService positionDBService;
        public PositionService(ILogger<PositionService> logger, IPositionDBService positionDBService)
        {
            this.logger = logger;
            this.positionDBService = positionDBService;
        }
        public async Task<string?> GetPositionNameById(string positionId)
        {
            var result = await positionDBService.GetPositionNameByIdAsync(positionId);
            if (result == null)
            {
                logger.LogWarning("Position with ID {PositionId} not found.", positionId);
                return null;
            }
            return result;
        }
    }
}
