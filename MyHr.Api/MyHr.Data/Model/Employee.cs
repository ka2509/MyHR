using MyHr.Data.Enum;

namespace MyHr.Data.Model
{
    public class Employee
    {
        public String Id { get; set; }
        public String OrganizationId { get; set; }
        public String FullName { get; set; }
        public Sex Sex { get; set; }
        public String SocialInsurance { get; set; }
        public DateTime Dob { get; set; }
        public String IdentityCardNumber { get; set; }
        public DateTime SocialInsuranceContributionDate { get; set; }
        public String PositionId { get; set; }
        public String ProfessionId { get; set; }
        public String Password { get; set; }
        public String? AllowanceId { get; set; }
    }
}
