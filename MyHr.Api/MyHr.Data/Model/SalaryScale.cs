namespace MyHr.Data.Model
{
    /// <summary>
    /// Ngạch lương - Nhiều trình độ (Profession) có thể cùng thuộc một ngạch lương
    /// Ví dụ: Công nhân hàn điện, Công nhân cơ điện đều thuộc Ngạch Công nhân
    /// </summary>
    public class SalaryScale
    {
        public String Id { get; set; } = string.Empty;
        public String Code { get; set; } = string.Empty;
        public String Name { get; set; } = string.Empty;
        public Int32 MaxGrade { get; set; }  // Số bậc lương tối đa trong ngạch này
    }
}
