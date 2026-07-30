using MyHr.Data.Enum;

namespace MyHr.Data.Dto
{
    public class EmployeeResponse
    {
        public String Id { get; set; }
        public String FullName { get; set; }
        public Sex Sex { get; set; }
        public String SocialInsurance { get; set; }
        public DateTime Dob { get; set; }
        public String IdentityCardNumber { get; set; }
        public DateTime SocialInsuranceContributionDate { get; set; }
        public String? OrganizationId { get; set; }
        public String? OrganizationName { get; set; }
        public Int32 OrganizationType { get; set; }
        public String? PositionName { get; set; }
        public String? ProfessionName { get; set; }
        public String? CurrentSalaryGrade { get; set; }
        public Decimal? SalaryCof { get; set; }
        public String? AllowanceId { get; set; }
        public String? AllowanceName { get; set; }
        public Decimal? AllowanceCoefficient { get; set; }
        public Decimal? TotalSalary { get; set; }
    }
}
