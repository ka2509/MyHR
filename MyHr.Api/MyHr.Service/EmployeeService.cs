using Microsoft.Extensions.Logging;
using MyHr.Data.Dto;
using MyHr.Data.Model;
using MyHr.Service.Interface;
using System.Security.Cryptography;
using System.Text;

namespace MyHr.Service
{
    internal class EmployeeService : IEmployeeService
    {
        private readonly ILogger<EmployeeService> logger;
        private readonly IEmployeeDBService employeeDBService;
        private readonly IOrganizationService organizationService;
        private readonly IOrganizationDBService organizationDBService;
        private readonly IProfessionService professionService;
        private readonly IProfessionDBService professionDBService;
        private readonly IPositionService positionService;
        private readonly IPositionDBService positionDBService;
        private readonly ISalaryGradeDBService salaryGradeDBService;
        private readonly IEmployeeSalaryDBService employeeSalaryDBService;
        private readonly IAllowanceDBService allowanceDBService;
        private readonly ISalaryCalculationService salaryCalculationService;

        public EmployeeService(ILogger<EmployeeService> logger,
            IEmployeeDBService employeeDBService,
            IOrganizationService organizationService,
            IOrganizationDBService organizationDBService,
            IProfessionService professionService,
            IProfessionDBService professionDBService,
            IPositionService positionService,
            IPositionDBService positionDBService,
            ISalaryGradeDBService salaryGradeDBService,
            IEmployeeSalaryDBService employeeSalaryDBService,
            IAllowanceDBService allowanceDBService,
            ISalaryCalculationService salaryCalculationService)
        {
            this.logger = logger;
            this.employeeDBService = employeeDBService;
            this.organizationService = organizationService;
            this.organizationDBService = organizationDBService;
            this.professionService = professionService;
            this.professionDBService = professionDBService;
            this.positionService = positionService;
            this.positionDBService = positionDBService;
            this.salaryGradeDBService = salaryGradeDBService;
            this.employeeSalaryDBService = employeeSalaryDBService;
            this.allowanceDBService = allowanceDBService;
            this.salaryCalculationService = salaryCalculationService;
        }

        public async Task<List<EmployeeResponse>> GetEmployeesByOrganization(String organizationId)
        {
            var employees = await employeeDBService.GetAllEmployees();
            var filteredEmployees = employees.Where(e => e.OrganizationId == organizationId).ToList();
            
            var result = new List<EmployeeResponse>();
            foreach (var employee in filteredEmployees)
            {
                var response = await ConvertEmployeeToResponse(employee);
                result.Add(response);
            }
            return result;
        }

        public async Task<EmployeeResponse?> GetEmployeeById(string employeeId)
        {
            var employee = await employeeDBService.GetEmployeeById(employeeId);
            if (employee == null)
            {
                this.logger.LogWarning("GetEmployeeById: No employee found with Id {EmployeeId}", employeeId);
                return null;
            }
            
            var response = await ConvertEmployeeToResponse(employee);
            return response;
        }

        public async Task<bool> ImportEmployee(ImportEmployeeDto request)
        {
            if (!ValidateEmployee(request))
            {
                this.logger.LogError("ImportEmployee failed: Invalid employee data for {FullName} (CCCD: {IdentityCardNumber}). " +
                    "GradeLevel: {GradeLevel}, FixedSalary: {FixedSalary}",
                    request.FullName, request.IdentityCardNumber, request.CurrentGradeLevel, request.FixedSalaryAmount);
                return false;
            }

            // 1. Xác định SalaryScaleId: Ưu tiên từ Position, nếu không có thì lấy từ Profession
            // Nếu nhân viên thuộc Ban điều hành => lương cố định (không có ngạch lương)
            var salaryScaleId = await GetEffectiveSalaryScaleId(request.OrganizationId, request.PositionId, request.ProfessionId);
            
            // 2. Create Employee first
            var employee = ConvertImportDtoToEmployee(request);
            var employeeResult = await employeeDBService.ImportEmployee(employee);
            
            if (!employeeResult)
            {
                this.logger.LogError("ImportEmployee failed: Could not save employee data for IdentityCardNumber {IdentityCardNumber}",
                    request.IdentityCardNumber);
                return false;
            }

            // 3. Handle salary based on salary type
            if (string.IsNullOrEmpty(salaryScaleId))
            {
                // Fixed salary for executives (Ban điều hành)
                if (request.FixedSalaryAmount == null || request.FixedSalaryAmount <= 0)
                {
                    this.logger.LogError("ImportEmployee failed: Executive requires FixedSalaryAmount for IdentityCardNumber {IdentityCardNumber}",
                        request.IdentityCardNumber);
                    return false;
                }

                var fixedSalary = new EmployeeSalary
                {
                    Id = Guid.NewGuid().ToString(),
                    EmployeeId = employee.Id,
                    SalaryGradeId = null, // No salary grade for executives
                    EffectiveFrom = request.SalaryEffectiveFrom,
                    EffectiveTo = null,
                    Reason = request.SalaryReason ?? "Lương cố định - Ban điều hành",
                    FixedSalaryAmount = request.FixedSalaryAmount.Value,
                    TotalSalary = request.FixedSalaryAmount.Value
                };

                var fixedSalaryResult = await employeeSalaryDBService.AddEmployeeSalary(fixedSalary);
                if (!fixedSalaryResult)
                {
                    this.logger.LogError("ImportEmployee failed: Could not save fixed salary for EmployeeId {EmployeeId}",
                        employee.Id);
                    return false;
                }

                this.logger.LogInformation("ImportEmployee succeeded for IdentityCardNumber: {IdentityCardNumber} (Fixed salary: {FixedSalaryAmount})",
                    request.IdentityCardNumber, request.FixedSalaryAmount);
                return true;
            }

            // 4. Find SalaryGrade by SalaryScaleId and GradeLevel (for employees with salary scales)
            var salaryGrade = await salaryGradeDBService.GetSalaryGradeByScaleAndLevel(
                salaryScaleId, request.CurrentGradeLevel);
            
            if (salaryGrade == null)
            {
                this.logger.LogError("ImportEmployee failed: SalaryGrade not found for SalaryScaleId {SalaryScaleId} and GradeLevel {GradeLevel}",
                    salaryScaleId, request.CurrentGradeLevel);
                return false;
            }

            // 5. Get allowance coefficient
            decimal? allowanceCoefficient = null;
            if (!string.IsNullOrEmpty(employee.AllowanceId))
            {
                var allowance = await allowanceDBService.GetAllowanceByIdAsync(employee.AllowanceId);
                if (allowance != null)
                {
                    allowanceCoefficient = allowance.Coefficient;
                }
            }

            // 6. Calculate total salary
            var totalSalary = salaryCalculationService.CalculateSalary(salaryGrade.SalaryCof, allowanceCoefficient);

            // 7. Create EmployeeSalary record
            var employeeSalary = new EmployeeSalary
            {
                Id = Guid.NewGuid().ToString(),
                EmployeeId = employee.Id,
                SalaryGradeId = salaryGrade.Id,
                EffectiveFrom = request.SalaryEffectiveFrom,
                EffectiveTo = null, // Current salary, no end date
                Reason = request.SalaryReason ?? "Import lần đầu",
                FixedSalaryAmount = null, // Not a fixed salary
                TotalSalary = totalSalary
            };

            var salaryResult = await employeeSalaryDBService.AddEmployeeSalary(employeeSalary);
            
            if (!salaryResult)
            {
                this.logger.LogError("ImportEmployee failed: Could not save salary data for EmployeeId {EmployeeId}",
                    employee.Id);
                return false;
            }

            this.logger.LogInformation("ImportEmployee succeeded for IdentityCardNumber: {IdentityCardNumber}, GradeLevel: {GradeLevel}, SalaryScaleId: {SalaryScaleId}",
                request.IdentityCardNumber, request.CurrentGradeLevel, salaryScaleId);
            return true;
        }

        public async Task<bool> AddEmployee(AddEmployeeDto request)
        {
            // Check if employee already exists
            var existingEmployee = await employeeDBService.GetEmployeeByIdentityCardNumber(request.IdentityCardNumber);
            if (existingEmployee != null)
            {
                this.logger.LogError("AddEmployee failed: Employee with IdentityCardNumber {IdentityCardNumber} already exists",
                    request.IdentityCardNumber);
                return false;
            }

            // Validate employee data
            if (!ValidateAddEmployee(request))
            {
                this.logger.LogError("AddEmployee failed: Invalid employee data for {FullName} (CCCD: {IdentityCardNumber})",
                    request.FullName, request.IdentityCardNumber);
                return false;
            }

            // Convert AddEmployeeDto to ImportEmployeeDto (they have the same structure)
            var importDto = new ImportEmployeeDto
            {
                FullName = request.FullName,
                Sex = request.Sex,
                SocialInsurance = request.SocialInsurance,
                Dob = request.Dob,
                IdentityCardNumber = request.IdentityCardNumber,
                SocialInsuranceContributionDate = request.SocialInsuranceContributionDate,
                OrganizationId = request.OrganizationId,
                PositionId = request.PositionId,
                ProfessionId = request.ProfessionId,
                AllowanceId = request.AllowanceId,
                CurrentGradeLevel = request.CurrentGradeLevel,
                SalaryEffectiveFrom = request.SalaryEffectiveFrom,
                SalaryReason = request.SalaryReason ?? "Tuyển dụng mới",
                FixedSalaryAmount = request.FixedSalaryAmount
            };

            // Use the existing ImportEmployee logic
            var result = await ImportEmployee(importDto);
            
            if (result)
            {
                this.logger.LogInformation("AddEmployee succeeded for {FullName} (CCCD: {IdentityCardNumber})",
                    request.FullName, request.IdentityCardNumber);
            }
            
            return result;
        }

        public async Task<bool> DeleteEmployee(string employeeId)
        {
            // Check if employee exists
            var employee = await employeeDBService.GetEmployeeById(employeeId);
            if (employee == null)
            {
                this.logger.LogError("DeleteEmployee failed: No employee found with Id {EmployeeId}", employeeId);
                return false;
            }

            this.logger.LogInformation("Deleting employee: {FullName} (CCCD: {IdentityCardNumber})",
                employee.FullName, employee.IdentityCardNumber);

            // Delete salary records first (foreign key constraint)
            var salaryDeleteResult = await employeeSalaryDBService.DeleteEmployeeSalaries(employeeId);
            if (!salaryDeleteResult)
            {
                this.logger.LogError("DeleteEmployee failed: Could not delete salary records for EmployeeId {EmployeeId}",
                    employeeId);
                return false;
            }

            // Delete employee
            var employeeDeleteResult = await employeeDBService.DeleteEmployee(employeeId);
            if (!employeeDeleteResult)
            {
                this.logger.LogError("DeleteEmployee failed: Could not delete employee with Id {EmployeeId}", employeeId);
                return false;
            }

            this.logger.LogInformation("DeleteEmployee succeeded for EmployeeId: {EmployeeId}", employeeId);
            return true;
        }

        public async Task<EmployeeResponse?> Login(string identityCardNumber, string password)
        {
            this.logger.LogInformation("Attempting login for IdentityCardNumber: {IdentityCardNumber}", identityCardNumber);
            if (String.IsNullOrEmpty(identityCardNumber) || String.IsNullOrEmpty(password))
            {
                this.logger.LogError("Login failed: IdentityCardNumber or password is empty.");
                return null;
            }
            Employee? employee = await employeeDBService.GetEmployeeByIdentityCardNumber(identityCardNumber);
            if (employee == null)
            {
                this.logger.LogError("Login failed: No employee found with IdentityCardNumber {IdentityCardNumber}", identityCardNumber);
                return null;
            }
            var hashedPassword = this.HashPassword(password);
            if (!String.Equals(employee.Password, hashedPassword))
            {
                this.logger.LogError("Login failed: Incorrect password for IdentityCardNumber {IdentityCardNumber}", identityCardNumber);
                return null;
            }
            this.logger.LogInformation("Login successful for IdentityCardNumber: {IdentityCardNumber}", identityCardNumber);
            var employeeResponse = await ConvertEmployeeToResponse(employee);
            return employeeResponse;
        }

        /// <summary>
        /// Xác định ngạch lương hiệu lực cho nhân viên.
        /// Logic: 
        ///   1. Nếu nhân viên thuộc "Ban điều hành" → lương cố định (return null)
        ///   2. Nếu Position có SalaryScaleId → dùng của Position
        ///   3. Nếu không → dùng của Profession
        /// </summary>
        private async Task<string?> GetEffectiveSalaryScaleId(string organizationId, string positionId, string professionId)
        {
            // Kiểm tra nếu nhân viên thuộc Ban điều hành → lương cố định
            var organization = await organizationDBService.GetOrganizationByIdAsync(organizationId);
            if (organization != null && organization.Name.Equals("Ban điều hành", StringComparison.OrdinalIgnoreCase))
            {
                this.logger.LogInformation("Employee belongs to Ban điều hành: {OrganizationName} -> Fixed salary (no salary scale)",
                    organization.Name);
                return null;
            }

            // Kiểm tra Position trước
            var position = await positionDBService.GetPositionByIdAsync(positionId);
            if (position != null && !string.IsNullOrEmpty(position.SalaryScaleId))
            {
                this.logger.LogInformation("Using SalaryScaleId from Position: {PositionName} -> {SalaryScaleId}",
                    position.Name, position.SalaryScaleId);
                return position.SalaryScaleId;
            }

            // Nếu Position không có SalaryScaleId, lấy từ Profession
            var profession = await professionDBService.GetProfessionByIdAsync(professionId);
            if (profession != null)
            {
                this.logger.LogInformation("Using SalaryScaleId from Profession: {ProfessionName} -> {SalaryScaleId}",
                    profession.Name, profession.SalaryScaleId);
                return profession.SalaryScaleId;
            }

            return null;
        }

        private Boolean ValidateEmployee(ImportEmployeeDto importEmployeeDto)
        {
            var basicValidation = !String.IsNullOrEmpty(importEmployeeDto.FullName) &&
                                 !String.IsNullOrEmpty(importEmployeeDto.SocialInsurance) &&
                                 !String.IsNullOrEmpty(importEmployeeDto.IdentityCardNumber) &&
                                 importEmployeeDto.Dob != default(DateTime) &&
                                 importEmployeeDto.SocialInsuranceContributionDate != default(DateTime) &&
                                 importEmployeeDto.SalaryEffectiveFrom != default(DateTime) &&
                                 !String.IsNullOrEmpty(importEmployeeDto.OrganizationId) &&
                                 !String.IsNullOrEmpty(importEmployeeDto.ProfessionId) &&
                                 !String.IsNullOrEmpty(importEmployeeDto.PositionId);
            
            if (!basicValidation)
                return false;
            
            // For fixed salary (executives), CurrentGradeLevel can be 0 and FixedSalaryAmount must be provided
            if (importEmployeeDto.FixedSalaryAmount.HasValue && importEmployeeDto.FixedSalaryAmount.Value > 0)
            {
                return true; // Fixed salary is valid
            }
            
            // For calculated salary, CurrentGradeLevel must be > 0
            return importEmployeeDto.CurrentGradeLevel > 0;
        }

        private Boolean ValidateAddEmployee(AddEmployeeDto addEmployeeDto)
        {
            var basicValidation = !String.IsNullOrEmpty(addEmployeeDto.FullName) &&
                                 !String.IsNullOrEmpty(addEmployeeDto.SocialInsurance) &&
                                 !String.IsNullOrEmpty(addEmployeeDto.IdentityCardNumber) &&
                                 addEmployeeDto.Dob != default(DateTime) &&
                                 addEmployeeDto.SocialInsuranceContributionDate != default(DateTime) &&
                                 addEmployeeDto.SalaryEffectiveFrom != default(DateTime) &&
                                 !String.IsNullOrEmpty(addEmployeeDto.OrganizationId) &&
                                 !String.IsNullOrEmpty(addEmployeeDto.ProfessionId) &&
                                 !String.IsNullOrEmpty(addEmployeeDto.PositionId);
            
            if (!basicValidation)
                return false;
            
            // For fixed salary (executives), CurrentGradeLevel can be 0 and FixedSalaryAmount must be provided
            if (addEmployeeDto.FixedSalaryAmount.HasValue && addEmployeeDto.FixedSalaryAmount.Value > 0)
            {
                return true; // Fixed salary is valid
            }
            
            // For calculated salary, CurrentGradeLevel must be > 0
            return addEmployeeDto.CurrentGradeLevel > 0;
        }

        private Employee ConvertImportDtoToEmployee(ImportEmployeeDto importEmployeeDto)
        {
            return new Employee
            {
                Id = Guid.NewGuid().ToString(),
                OrganizationId = importEmployeeDto.OrganizationId,
                FullName = importEmployeeDto.FullName,
                Sex = importEmployeeDto.Sex,
                SocialInsurance = importEmployeeDto.SocialInsurance,
                Dob = importEmployeeDto.Dob,
                IdentityCardNumber = importEmployeeDto.IdentityCardNumber,
                SocialInsuranceContributionDate = importEmployeeDto.SocialInsuranceContributionDate,
                PositionId = importEmployeeDto.PositionId,
                ProfessionId = importEmployeeDto.ProfessionId,
                AllowanceId = importEmployeeDto.AllowanceId,
                Password = this.HashPassword(this.GeneratePassword())
            };
        }

        private async Task<EmployeeResponse> ConvertEmployeeToResponse(Employee employee)
        {
            // Xác định SalaryScaleId hiệu lực (kiểm tra Ban điều hành, ưu tiên Position, sau đó Profession)
            var salaryScaleId = await GetEffectiveSalaryScaleId(employee.OrganizationId, employee.PositionId, employee.ProfessionId);
            
            // Get current salary grade info
            var currentSalary = await employeeSalaryDBService.GetCurrentSalaryByEmployeeId(employee.Id);
            string? currentSalaryGradeDisplay = null;
            decimal? salaryCof = null;
            decimal? totalSalary = null;
            
            if (currentSalary != null)
            {
                // Always get total salary if it exists
                totalSalary = currentSalary.TotalSalary;
                
                // For calculated salary (with salary scale)
                if (!string.IsNullOrEmpty(salaryScaleId) && !string.IsNullOrEmpty(currentSalary.SalaryGradeId))
                {
                    var salaryGrades = await salaryGradeDBService.GetSalaryGradesByScaleId(salaryScaleId);
                    var currentGrade = salaryGrades.FirstOrDefault(g => g.Id == currentSalary.SalaryGradeId);
                    if (currentGrade != null)
                    {
                        var maxGrade = salaryGrades.Max(g => g.GradeLevel);
                        currentSalaryGradeDisplay = $"{currentGrade.GradeLevel}/{maxGrade}";
                        salaryCof = currentGrade.SalaryCof;
                    }
                }
                // For fixed salary (executives in Ban điều hành)
                else if (string.IsNullOrEmpty(salaryScaleId))
                {
                    currentSalaryGradeDisplay = "Lương cố định";
                }
            }
            else if (string.IsNullOrEmpty(salaryScaleId))
            {
                // Fixed salary position but no salary record yet
                currentSalaryGradeDisplay = "Lương cố định";
            }

            // Get organization info
            var organization = await organizationDBService.GetOrganizationByIdAsync(employee.OrganizationId);

            // Get allowance info
            Allowance? allowance = null;
            if (!string.IsNullOrEmpty(employee.AllowanceId))
            {
                allowance = await allowanceDBService.GetAllowanceByIdAsync(employee.AllowanceId);
            }

            return new EmployeeResponse
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Sex = employee.Sex,
                SocialInsurance = employee.SocialInsurance,
                Dob = employee.Dob,
                IdentityCardNumber = employee.IdentityCardNumber,
                SocialInsuranceContributionDate = employee.SocialInsuranceContributionDate,
                OrganizationId = employee.OrganizationId,
                OrganizationName = organization?.Name,
                OrganizationType = organization != null ? (int)organization.Type : 0,
                PositionName = await positionService.GetPositionNameById(employee.PositionId),
                ProfessionName = await professionService.GetProfessionNameById(employee.ProfessionId),
                CurrentSalaryGrade = currentSalaryGradeDisplay,
                SalaryCof = salaryCof,
                AllowanceId = employee.AllowanceId,
                AllowanceName = allowance?.Name,
                AllowanceCoefficient = allowance?.Coefficient,
                TotalSalary = totalSalary
            };
        }

        private String HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA512.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        private String GeneratePassword()
        {
            return "congtythuyloibac";
        }
    }
}
