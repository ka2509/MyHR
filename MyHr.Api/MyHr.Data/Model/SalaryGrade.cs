namespace MyHr.Data.Model
{
    /// <summary>
    /// Bậc lương trong một ngạch lương
    /// </summary>
    public class SalaryGrade
    {
        public String Id { get; set; } = string.Empty;
        public String SalaryScaleId { get; set; } = string.Empty;  // Khóa ngoại đến ngạch lương
        public Int32 GradeLevel { get; set; }
        public Decimal SalaryCof { get; set; }
        public Decimal BaseSalary { get; set; }
        
        /// <summary>
        /// Số tháng để tăng lên bậc tiếp theo (cho Nam)
        /// </summary>
        public Int32 PromotionMonthsMale { get; set; }
        
        /// <summary>
        /// Số tháng để tăng lên bậc tiếp theo (cho Nữ)
        /// </summary>
        public Int32 PromotionMonthsFemale { get; set; }
    }
}
