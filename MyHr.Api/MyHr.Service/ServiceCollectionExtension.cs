using Microsoft.Extensions.DependencyInjection;
using MyHr.Service.DatabaseService;
using MyHr.Service.Interface;

namespace MyHr.Service
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Employee services
            services.AddScoped<IEmployeeDBService, EmployeeDatabaseService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            
            // Organization services
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IOrganizationDBService, OrganizationDatabaseService>();
            
            // Profession services
            services.AddScoped<IProfessionService, ProfessionService>();
            services.AddScoped<IProfessionDBService, ProfessionDatabaseService>();
            
            // Position services
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IPositionDBService, PositionDatabaseService>();
            
            // Salary services
            services.AddScoped<ISalaryScaleDBService, SalaryScaleDatabaseService>();
            services.AddScoped<ISalaryGradeDBService, SalaryGradeDatabaseService>();
            services.AddScoped<IEmployeeSalaryDBService, EmployeeSalaryDatabaseService>();
            
            // Allowance services
            services.AddScoped<IAllowanceDBService, AllowanceDatabaseService>();
            
            // Salary calculation service
            services.AddScoped<ISalaryCalculationService, SalaryCalculationService>();
            
            return services;
        }
    }
}
