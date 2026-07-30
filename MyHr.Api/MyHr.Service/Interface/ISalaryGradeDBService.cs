using MyHr.Data.Model;

namespace MyHr.Service.Interface
{
    public interface ISalaryGradeDBService
    {
        Task<SalaryGrade?> GetSalaryGradeByIdAsync(String salaryGradeId);
        Task<SalaryGrade?> GetSalaryGradeByScaleAndLevel(String salaryScaleId, Int32 gradeLevel);
        Task<List<SalaryGrade>> GetSalaryGradesByScaleId(String salaryScaleId);
    }
}
