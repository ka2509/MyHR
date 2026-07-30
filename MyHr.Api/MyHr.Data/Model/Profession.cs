namespace MyHr.Data.Model
{
    /// <summary>
    /// Trình độ/Bằng cấp của nhân viên
    /// Ví dụ: Công nhân hàn điện, Công nhân cơ điện, Kỹ sư thủy lợi
    /// </summary>
    public class Profession
    {
        public String Id { get; set; } = string.Empty;
        public String Code { get; set; } = string.Empty;
        public String Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Khóa ngoại đến ngạch lương (có thể NULL nếu chưa xác định hoặc theo Position)
        /// </summary>
        public String? SalaryScaleId { get; set; }
    }
}
