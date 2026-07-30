using MyHr.Data.Enum;

namespace MyHr.DataImporter
{
    /// <summary>
    /// Represents a row from the Excel file
    /// </summary>
    public class ExcelEmployeeRow
    {
        public int STT { get; set; }
        public string HoVaTen { get; set; } = string.Empty;
        public bool GioiTinhNu { get; set; }  // x = Nữ
        public string MaSoBHXH { get; set; } = string.Empty;
        public DateTime NgayThangNamSinh { get; set; }
        public string SoCCCD { get; set; } = string.Empty;
        public DateTime ThoiGianDongBHXH { get; set; }
        public string ChuyenMonNghiepVu { get; set; } = string.Empty;  // Position name
        public string TrinhDo { get; set; } = string.Empty;  // Profession name
        public int BacMoi { get; set; }  // Current grade level
        public decimal HeSoMoi { get; set; }
        public decimal? PCCV { get; set; }  // Phụ cấp công việc (Job Allowance) - 0.2, 0.1
        public decimal? PCTN { get; set; }  // Phụ cấp trách nhiệm (Responsibility Allowance) - 0.5, 0.3
        public decimal LuongMoi { get; set; }
        
        // Sheet and section info for organization mapping
        public string SheetName { get; set; } = string.Empty;
        public string? SectionHeader { get; set; }  // Section header row (e.g., "Phòng Kế hoạch", "Cụm Trà Lĩnh")
        
        // Mapped IDs (will be resolved from lookup tables)
        public string? OrganizationId { get; set; }
        public string? PositionId { get; set; }
        public string? ProfessionId { get; set; }
    }
}
