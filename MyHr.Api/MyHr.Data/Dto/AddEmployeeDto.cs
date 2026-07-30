using MyHr.Data.Enum;

namespace MyHr.Data.Dto
{
    public class AddEmployeeDto
    {
        // Employee basic info
        public String FullName { get; set; }
        public Sex Sex { get; set; }
        public String SocialInsurance { get; set; }
        public DateTime Dob { get; set; }
        public String IdentityCardNumber { get; set; }
        public DateTime SocialInsuranceContributionDate { get; set; }
        public String OrganizationId { get; set; }
        public String PositionId { get; set; }
        public String ProfessionId { get; set; }
        public String? AllowanceId { get; set; }  // Phụ cấp (có thể null)

        // Current salary grade info
        public Int32 CurrentGradeLevel { get; set; }  // Bậc lương hiện tại (1, 2, 3...)
        public DateTime SalaryEffectiveFrom { get; set; }  // Ngày hiệu lực bậc lương
        public String? SalaryReason { get; set; }  // Lý do (có thể null khi thêm mới)
        
        // Fixed salary for executives (Ban điều hành)
        public Decimal? FixedSalaryAmount { get; set; }  // Lương cố định (chỉ dùng cho Ban điều hành)
    }
}
