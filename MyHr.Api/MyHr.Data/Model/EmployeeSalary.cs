namespace MyHr.Data.Model
{
    public class EmployeeSalary
    {
        public String Id { get; set; }
        public String EmployeeId { get; set; }
        public String? SalaryGradeId { get; set; } // Nullable for executives with fixed salary
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; } // = null => effect to current
        public String? Reason { get; set; }
        public Decimal? FixedSalaryAmount { get; set; } // Only for executives (when SalaryGradeId IS NULL)
        public Decimal TotalSalary { get; set; } // Calculated total salary or fixed amount
    }
}
