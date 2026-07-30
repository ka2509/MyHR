using MyHr.Data.Model;

namespace MyHr.Service.Interface
{
    public interface IPositionDBService
    {
        Task<String?> GetPositionNameByIdAsync(String positionId);
        Task<Position?> GetPositionByIdAsync(String positionId);
        Task<List<Position>> GetAllPositionsAsync();
    }
}
