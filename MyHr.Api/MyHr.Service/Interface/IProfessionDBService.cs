using MyHr.Data.Model;

namespace MyHr.Service.Interface
{
    public interface IProfessionDBService
    {
        Task<String?> GetProfessionNameByIdAsync(String professionId);
        Task<Profession?> GetProfessionByIdAsync(String professionId);
        Task<List<Profession>> GetAllProfessionsAsync();
    }
}
