namespace MyHr.Data.Model
{
    public class Position
    {
        public String Id { get; set; } = string.Empty;
        public String Code { get; set; } = string.Empty;
        public String Name { get; set; } = string.Empty;
        public Decimal AllowanceCof { get; set; }
        public Boolean IsManagement { get; set; }
        
        /// <summary>
        /// Ngạch lương gắn với chức vụ (chỉ áp dụng cho các chức vụ quản lý).
        /// Nếu NULL, ngạch lương sẽ lấy từ Profession của nhân viên.
        /// </summary>
        public String? SalaryScaleId { get; set; }
    }
}
