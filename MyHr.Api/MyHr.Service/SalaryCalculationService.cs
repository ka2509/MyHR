using Microsoft.Extensions.Logging;
using MyHr.Service.Interface;

namespace MyHr.Service
{
    internal class SalaryCalculationService : ISalaryCalculationService
    {
        // Fixed base salary as per user requirement
        private const decimal BASE_SALARY = 2340000m;
        
        private readonly ILogger<SalaryCalculationService> logger;
        private readonly IEmployeeDBService employeeDBService;
        private readonly IEmployeeSalaryDBService employeeSalaryDBService;
        private readonly ISalaryGradeDBService salaryGradeDBService;
        private readonly IAllowanceDBService allowanceDBService;

        public SalaryCalculationService(
            ILogger<SalaryCalculationService> logger,
            IEmployeeDBService employeeDBService,
            IEmployeeSalaryDBService employeeSalaryDBService,
            ISalaryGradeDBService salaryGradeDBService,
            IAllowanceDBService allowanceDBService)
        {
            this.logger = logger;
            this.employeeDBService = employeeDBService;
            this.employeeSalaryDBService = employeeSalaryDBService;
            this.salaryGradeDBService = salaryGradeDBService;
            this.allowanceDBService = allowanceDBService;
        }

        /// <summary>
        /// Calculate total salary for an employee
        /// Formula: TotalSalary = BASE_SALARY * (AllowanceCoefficient + SalaryCoefficient)
        /// Where BASE_SALARY = 2,340,000 (fixed)
        /// </summary>
        public async Task<decimal> CalculateTotalSalaryAsync(string employeeId)
        {
            // Get employee
            var employees = await employeeDBService.GetAllEmployees();
            var employee = employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                logger.LogWarning($"Employee not found: {employeeId}");
                return 0;
            }

            // Get current salary grade
            var currentEmployeeSalary = await employeeSalaryDBService.GetCurrentSalaryByEmployeeId(employeeId);
            if (currentEmployeeSalary == null)
            {
                logger.LogWarning($"No current salary found for employee: {employeeId}");
                return 0;
            }

            // Get salary grade details
            var salaryGrade = await salaryGradeDBService.GetSalaryGradeByIdAsync(currentEmployeeSalary.SalaryGradeId);
            if (salaryGrade == null)
            {
                logger.LogWarning($"Salary grade not found: {currentEmployeeSalary.SalaryGradeId}");
                return 0;
            }

            // Get allowance coefficient (if employee has allowance)
            decimal allowanceCoefficient = 0;
            if (!string.IsNullOrEmpty(employee.AllowanceId))
            {
                var allowance = await allowanceDBService.GetAllowanceByIdAsync(employee.AllowanceId);
                if (allowance != null)
                {
                    allowanceCoefficient = allowance.Coefficient;
                }
            }

            // Calculate total salary
            return CalculateSalary(salaryGrade.SalaryCof, allowanceCoefficient);
        }

        /// <summary>
        /// Calculate salary with specific parameters
        /// Formula: TotalSalary = BASE_SALARY * (allowanceCoefficient + salaryCoefficient)
        /// </summary>
        public decimal CalculateSalary(decimal salaryCoefficient, decimal? allowanceCoefficient)
        {
            var totalCoefficient = salaryCoefficient + (allowanceCoefficient ?? 0);
            var totalSalary = BASE_SALARY * totalCoefficient;
            
            logger.LogDebug($"Calculating salary: BASE_SALARY({BASE_SALARY}) * (SalaryCof({salaryCoefficient}) + AllowanceCof({allowanceCoefficient ?? 0})) = {totalSalary}");
            
            return Math.Round(totalSalary, 0); // Round to nearest whole number
        }

        /// <summary>
        /// Recalculate and update total salary for all employees
        /// This method iterates through all employees and updates their current salary records
        /// </summary>
        public async Task RecalculateAllSalariesAsync()
        {
            logger.LogInformation("Starting recalculation of all employee salaries...");
            
            var employees = await employeeDBService.GetAllEmployees();
            int successCount = 0;
            int failureCount = 0;

            foreach (var employee in employees)
            {
                try
                {
                    var calculatedSalary = await CalculateTotalSalaryAsync(employee.Id);
                    
                    // Get current salary record
                    var currentSalary = await employeeSalaryDBService.GetCurrentSalaryByEmployeeId(employee.Id);
                    if (currentSalary != null && calculatedSalary > 0)
                    {
                        currentSalary.TotalSalary = calculatedSalary;
                        // Note: You'll need to add an Update method to IEmployeeSalaryDBService
                        // For now, just log the calculated value
                        logger.LogInformation($"Employee {employee.FullName} ({employee.Id}): Calculated salary = {calculatedSalary:N0} VND");
                        successCount++;
                    }
                    else
                    {
                        logger.LogWarning($"Could not calculate salary for {employee.FullName} ({employee.Id})");
                        failureCount++;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error calculating salary for employee {employee.Id}");
                    failureCount++;
                }
            }

            logger.LogInformation($"Salary recalculation completed. Success: {successCount}, Failures: {failureCount}");
        }
    }
}
