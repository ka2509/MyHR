namespace MyHr.Service.Interface
{
    public interface ISalaryCalculationService
    {
        /// <summary>
        /// Calculate total salary for an employee
        /// Formula: TotalSalary = BASE_SALARY * (AllowanceCoefficient + SalaryCoefficient)
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <returns>Calculated total salary</returns>
        Task<decimal> CalculateTotalSalaryAsync(string employeeId);
        
        /// <summary>
        /// Calculate total salary with specific parameters
        /// Formula: TotalSalary = BASE_SALARY * (allowanceCoefficient + salaryCoefficient)
        /// </summary>
        /// <param name="salaryCoefficient">Salary coefficient from SalaryGrade</param>
        /// <param name="allowanceCoefficient">Allowance coefficient (can be null)</param>
        /// <returns>Calculated total salary</returns>
        decimal CalculateSalary(decimal salaryCoefficient, decimal? allowanceCoefficient);
        
        /// <summary>
        /// Recalculate and update total salary for all employees
        /// </summary>
        Task RecalculateAllSalariesAsync();
    }
}
