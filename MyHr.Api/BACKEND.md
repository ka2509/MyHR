# MyHR – Backend Reference

> **Purpose of this file**: Comprehensive reference for generating consistent, precise backend code for the MyHR project. Read this before implementing any backend task.

---

## Tech Stack

| Tool | Version | Purpose |
|------|---------|---------|
| .NET | 10.0 | Runtime |
| ASP.NET Core | 10.0 | Web API framework |
| Microsoft.Data.SqlClient | 6.1.4 | Database access (raw SQL, no ORM) |
| Swashbuckle | 10.1.0 | Swagger UI at `/swagger` |

**No ORM** (no Entity Framework). All database access uses raw SQL with parameterized `SqlCommand`.

---

## Solution Structure

```
MyHr.Api/                           ← Solution root (railway.json, Dockerfile here)
├── MyHr.Api/                       ← ASP.NET Core Web API project
│   ├── Program.cs                  ← App bootstrap, DI, middleware
│   ├── appsettings.json            ← Config: connection string, CORS origins
│   ├── Controllers/
│   │   ├── EmployeeController.cs
│   │   ├── AllowanceController.cs
│   │   ├── OrganizationController.cs
│   │   ├── PositionController.cs
│   │   └── ProfessionController.cs
│   └── Properties/launchSettings.json
│
├── MyHr.Service/                   ← Business logic + data access project
│   ├── ServiceCollectionExtension.cs   ← Registers all DI services
│   ├── EmployeeService.cs
│   ├── OrganizationService.cs
│   ├── PositionService.cs
│   ├── ProfessionService.cs
│   ├── SalaryCalculationService.cs
│   ├── DatabaseService/
│   │   ├── DatabaseService.cs          ← Abstract base: GetSqlConnectionAsync()
│   │   ├── EmployeeDatabaseService.cs
│   │   ├── EmployeeSalaryDatabaseService.cs
│   │   ├── AllowanceDatabaseService.cs
│   │   ├── OrganizationDatabaseService.cs
│   │   ├── PositionDatabaseService.cs
│   │   ├── ProfessionDatabaseService.cs
│   │   ├── SalaryGradeDatabaseService.cs
│   │   └── SalaryScaleDatabaseService.cs
│   └── Interface/
│       ├── IEmployeeService.cs
│       ├── IEmployeeDBService.cs
│       ├── IEmployeeSalaryDBService.cs
│       ├── IOrganizationService.cs
│       ├── IOrganizationDBService.cs
│       ├── IPositionService.cs
│       ├── IPositionDBService.cs
│       ├── IProfessionService.cs
│       ├── IProfessionDBService.cs
│       ├── ISalaryCalculationService.cs
│       ├── ISalaryGradeDBService.cs
│       ├── ISalaryScaleDBService.cs
│       └── IAllowanceDBService.cs
│
├── MyHr.Data/                      ← Shared models, DTOs, enums (no logic)
│   ├── Model/
│   │   ├── Employee.cs
│   │   ├── EmployeeSalary.cs
│   │   ├── Organization.cs
│   │   ├── Position.cs
│   │   ├── Profession.cs
│   │   ├── SalaryGrade.cs
│   │   ├── SalaryScale.cs
│   │   └── Allowance.cs
│   ├── Dto/
│   │   ├── EmployeeResponse.cs
│   │   ├── AddEmployeeDto.cs
│   │   ├── ImportEmployeeDto.cs
│   │   ├── LoginRequest.cs
│   │   └── OrganizationResponse.cs
│   └── Enum/
│       ├── Sex.cs
│       ├── OrganizationType.cs
│       └── AllowanceType.cs
│
├── MyHr.DataImporter/              ← One-time Excel import utility (not part of API)
│   ├── Program.cs
│   └── ExcelEmployeeRow.cs
│
└── Data/                           ← SQL scripts for database setup
    ├── CreateDatabase.sql
    ├── InsertSalaryScales.sql
    ├── InsertSalaryGrades.sql
    ├── InsertAllowances.sql
    ├── InsertOrganizations.sql
    ├── InsertProfessions.sql
    └── InsertPositions.sql
```

---

## Architecture Layers

```
HTTP Request
     ↓
[Controller]  — receives HTTP, calls IService, returns IActionResult
     ↓
[IService / Service]  — business logic, orchestrates DB calls
     ↓
[IDBService / DatabaseService]  — raw SQL queries, returns domain models
     ↓
SQL Server (Azure SQL in production, LocalDB in dev)
```

**Dependency direction**: `Api → Service → Data`. `MyHr.Data` has no dependencies.

**All services are registered as `Scoped`** (per HTTP request) in `ServiceCollectionExtension.cs`.

---

## Configuration

### `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=myhr;Integrated Security=true;TrustServerCertificate=true"
  },
  "AllowedOrigins": [
    "http://localhost:5173",
    "http://localhost:3000"
  ]
}
```

### Connection String Resolution (in `DatabaseService.cs`)
Reads from environment variable first, then falls back to `appsettings.json` default:
```csharp
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? _defaultConnectionString;
```

**Production (Railway)**: Set `ConnectionStrings__DefaultConnection` env var to the Azure SQL connection string.

### CORS
Reads `AllowedOrigins` array from config. Override in production by setting:
- `AllowedOrigins__0 = https://your-frontend.vercel.app`

---

## API Endpoints

### `EmployeeController` — `/api/Employees`

| Method | Path | Request Body | Returns | Notes |
|--------|------|-------------|---------|-------|
| GET | `/organization/{organizationId}` | — | `List<EmployeeResponse>` | All employees in org |
| GET | `/{employeeId}` | — | `EmployeeResponse` | 404 if not found |
| POST | `/Login` | `LoginRequest` | `EmployeeResponse` | 401 if wrong credentials |
| POST | `/` | `ImportEmployeeDto` | 200/400 | Bulk import (Excel importer) |
| POST | `/add` | `AddEmployeeDto` | `{message}` | Single add from UI |
| DELETE | `/{employeeId}` | — | `{message}` | Deletes employee + salary records |

### `AllowanceController` — `/api/Allowances`

| Method | Path | Returns |
|--------|------|---------|
| GET | `/` | `List<Allowance>` |
| GET | `/{id}` | `Allowance` |

### `OrganizationController` — `/api/Organizations`

| Method | Path | Returns | Notes |
|--------|------|---------|-------|
| GET | `/main` | `List<OrganizationResponse>` | Type 0 (TongCongTy) + Type 1 (ChiNhanh) |
| GET | `/{parentId}/sub` | `List<OrganizationResponse>` | Type 2,3,4 under parentId |

### `PositionController` — `/api/Positions`

| Method | Path | Returns |
|--------|------|---------|
| GET | `/` | `List<Position>` |
| GET | `/{id}` | `Position` |

### `ProfessionController` — `/api/Professions`

| Method | Path | Returns |
|--------|------|---------|
| GET | `/` | `List<Profession>` |
| GET | `/{id}` | `Profession` |

**JSON serialization**: all properties serialized as **camelCase** (configured in `Program.cs`). Enums serialized as **integers**.

---

## Database Schema

### `Organization`
```sql
Id          NVARCHAR(50) PK
Name        NVARCHAR(200) NOT NULL
Type        INT NOT NULL          -- OrganizationType enum (0–4)
ParentId    NVARCHAR(50) NULL     -- FK → Organization(Id), self-reference
```
Hierarchy: `TongCongTy(0) → ChiNhanh(1) → Phong(2) → Cum(3) → To(4)`

### `SalaryScale` (Ngạch lương)
```sql
Id          NVARCHAR(50) PK
Code        NVARCHAR(50)
Name        NVARCHAR(200)
MaxGrade    INT                   -- Max number of grade levels in this scale
```

### `SalaryGrade` (Bậc lương)
```sql
Id                    NVARCHAR(50) PK
SalaryScaleId         NVARCHAR(50) FK → SalaryScale
GradeLevel            INT                    -- 1 to MaxGrade
SalaryCof             DECIMAL(10,2)          -- Salary coefficient
BaseSalary            DECIMAL(18,0)          -- Pre-calculated base (informational)
PromotionMonthsMale   INT DEFAULT 24
PromotionMonthsFemale INT DEFAULT 24
UNIQUE (SalaryScaleId, GradeLevel)
```

### `Position` (Chức vụ)
```sql
Id              NVARCHAR(50) PK
Code            NVARCHAR(50)
Name            NVARCHAR(200)
AllowanceCof    DECIMAL(10,2) DEFAULT 0
IsManagement    BIT DEFAULT 0             -- 1 = top management (Giám đốc, etc.)
SalaryScaleId   NVARCHAR(50) NULL FK → SalaryScale
-- NULL means: use employee's Profession scale instead
```

### `Profession` (Trình độ/Bằng cấp)
```sql
Id              NVARCHAR(50) PK
Code            NVARCHAR(50)
Name            NVARCHAR(200)
SalaryScaleId   NVARCHAR(50) NULL FK → SalaryScale
```

### `Allowance` (Phụ cấp)
```sql
Id              NVARCHAR(50) PK
Type            INT NOT NULL   -- 1=Responsibility, 2=Job
Level           INT NOT NULL   -- 1=higher, 2=lower
Name            NVARCHAR(200)
Coefficient     DECIMAL(10,2)  -- 0.5, 0.3, 0.2, 0.1
UNIQUE (Type, Level)
```
4 records: Responsibility(0.5), Responsibility(0.3), Job(0.2), Job(0.1)

### `Employee`
```sql
Id                              NVARCHAR(50) PK
OrganizationId                  NVARCHAR(50) FK → Organization
FullName                        NVARCHAR(200)
Sex                             INT           -- 0=Female, 1=Male
SocialInsurance                 NVARCHAR(50) UNIQUE
Dob                             DATE
IdentityCardNumber              NVARCHAR(20) UNIQUE
SocialInsuranceContributionDate DATE
PositionId                      NVARCHAR(50) FK → Position
ProfessionId                    NVARCHAR(50) FK → Profession
Password                        NVARCHAR(500) -- SHA512 hashed
AllowanceId                     NVARCHAR(50) NULL FK → Allowance
```

### `EmployeeSalary` (Lịch sử lương)
```sql
Id                  NVARCHAR(50) PK
EmployeeId          NVARCHAR(50) FK → Employee
SalaryGradeId       NVARCHAR(50) NULL FK → SalaryGrade
EffectiveFrom       DATE
EffectiveTo         DATE NULL        -- NULL = current active record
Reason              NVARCHAR(500) NULL
FixedSalaryAmount   DECIMAL(18,0) NULL  -- Only for executives
TotalSalary         DECIMAL(18,0) DEFAULT 0
-- CHECK: (SalaryGradeId NOT NULL AND FixedSalaryAmount IS NULL) OR
--        (SalaryGradeId IS NULL AND FixedSalaryAmount NOT NULL)
```
**Current salary record**: `WHERE EffectiveTo IS NULL`

---

## Data Models (`MyHr.Data`)

### Enums

```csharp
// MyHr.Data.Enum
enum Sex { Female = 0, Male = 1 }

enum OrganizationType { TongCongTy = 0, ChiNhanh = 1, Phong = 2, Cum = 3, To = 4 }

enum AllowanceType { Responsibility = 1, Job = 2 }
```

### Domain Models (`MyHr.Data.Model`)

```csharp
class Employee {
    string Id, OrganizationId, FullName, SocialInsurance, IdentityCardNumber, PositionId, ProfessionId, Password;
    Sex Sex;
    DateTime Dob, SocialInsuranceContributionDate;
    string? AllowanceId;
}

class EmployeeSalary {
    string Id, EmployeeId;
    string? SalaryGradeId;      // null for fixed-salary executives
    DateTime EffectiveFrom;
    DateTime? EffectiveTo;      // null = active record
    string? Reason;
    decimal? FixedSalaryAmount; // only when SalaryGradeId is null
    decimal TotalSalary;
}

class Organization {
    string Id, Name;
    OrganizationType Type;
    string? ParentId;
}

class Position {
    string Id, Code, Name;
    decimal AllowanceCof;
    bool IsManagement;
    string? SalaryScaleId;      // null = use Profession's scale
}

class Profession {
    string Id, Code, Name;
    string? SalaryScaleId;
}

class SalaryGrade {
    string Id, SalaryScaleId;
    int GradeLevel;
    decimal SalaryCof, BaseSalary;
    int PromotionMonthsMale, PromotionMonthsFemale;
}

class SalaryScale {
    string Id, Code, Name;
    int MaxGrade;
}

class Allowance {
    string Id, Name;
    AllowanceType Type;
    int Level;
    decimal Coefficient;
}
```

### DTOs (`MyHr.Data.Dto`)

```csharp
// Returned by Login and GetEmployee* endpoints
class EmployeeResponse {
    string Id, FullName, SocialInsurance, IdentityCardNumber;
    Sex Sex;
    DateTime Dob, SocialInsuranceContributionDate;
    string? OrganizationId, OrganizationName;
    int OrganizationType;
    string? PositionName, ProfessionName;
    string? CurrentSalaryGrade;  // grade level as string
    decimal? SalaryCof;
    string? AllowanceId, AllowanceName;
    decimal? AllowanceCoefficient;
    decimal? TotalSalary;
}

// Used for single employee add from UI (POST /add)
class AddEmployeeDto {
    string FullName, SocialInsurance, IdentityCardNumber, OrganizationId, PositionId, ProfessionId;
    Sex Sex;
    DateTime Dob, SocialInsuranceContributionDate, SalaryEffectiveFrom;
    string? AllowanceId, SalaryReason;
    int CurrentGradeLevel;
    decimal? FixedSalaryAmount;  // only for executives
}

// Used for bulk import via DataImporter (POST /)
// Identical structure to AddEmployeeDto
class ImportEmployeeDto { /* same fields as AddEmployeeDto */ }

class LoginRequest {
    string IdentityCardNumber, Password;
}

class OrganizationResponse {
    string Id, Name;
    int Type;          // OrganizationType as int
    string? ParentId;
}
```

---

## Service Interfaces

### `IEmployeeService` (business logic layer)
```csharp
Task<EmployeeResponse?> Login(string identityCardNumber, string password);
Task<List<EmployeeResponse>> GetEmployeesByOrganization(string organizationId);
Task<EmployeeResponse?> GetEmployeeById(string employeeId);
Task<bool> ImportEmployee(ImportEmployeeDto request);
Task<bool> AddEmployee(AddEmployeeDto request);
Task<bool> DeleteEmployee(string employeeId);
```

### `IEmployeeDBService` (data access)
```csharp
Task<Employee?> GetEmployeeByIdentityCardNumber(string identityCardNumber);
Task<Employee?> GetEmployeeById(string employeeId);
Task<List<Employee>> GetAllEmployees();
Task<bool> ImportEmployee(Employee employee);
Task<bool> DeleteEmployee(string employeeId);
```

### `IEmployeeSalaryDBService`
```csharp
Task<bool> AddEmployeeSalary(EmployeeSalary employeeSalary);
Task<EmployeeSalary?> GetCurrentSalaryByEmployeeId(string employeeId);   // EffectiveTo IS NULL
Task<List<EmployeeSalary>> GetSalaryHistoryByEmployeeId(string employeeId);
Task<bool> DeleteEmployeeSalaries(string employeeId);
```

### `IOrganizationService`
```csharp
Task<string?> GetOrganizationNameById(string orgId);
Task<List<OrganizationResponse>> GetMainOrganizations();   // Type 0,1
Task<List<OrganizationResponse>> GetSubOrganizations(string parentId); // Type 2,3,4
```

### `IOrganizationDBService`
```csharp
Task<string?> GetOrganizationNameByIdAsync(string orgId);
Task<List<Organization>> GetAllOrganizationsAsync();
Task<Organization?> GetOrganizationByIdAsync(string orgId);
```

### `IPositionService` / `IProfessionService`
```csharp
Task<string?> GetPositionNameById(string positionId);
Task<string?> GetProfessionNameById(string professionId);
```

### `IPositionDBService` / `IProfessionDBService`
```csharp
Task<string?> GetPositionNameByIdAsync(string id);
Task<Position?> GetPositionByIdAsync(string id);
Task<List<Position>> GetAllPositionsAsync();
// (same shape for IProfessionDBService)
```

### `ISalaryCalculationService`
```csharp
Task<decimal> CalculateTotalSalaryAsync(string employeeId);
decimal CalculateSalary(decimal salaryCoefficient, decimal? allowanceCoefficient);
Task RecalculateAllSalariesAsync();
```

### `ISalaryGradeDBService`
```csharp
Task<SalaryGrade?> GetSalaryGradeByIdAsync(string salaryGradeId);
Task<SalaryGrade?> GetSalaryGradeByScaleAndLevel(string salaryScaleId, int gradeLevel);
Task<List<SalaryGrade>> GetSalaryGradesByScaleId(string salaryScaleId);
```

### `ISalaryScaleDBService`
```csharp
Task<SalaryScale?> GetSalaryScaleByIdAsync(string salaryScaleId);
Task<List<SalaryScale>> GetAllSalaryScalesAsync();
```

### `IAllowanceDBService`
```csharp
Task<Allowance?> GetAllowanceByIdAsync(string allowanceId);
Task<List<Allowance>> GetAllAllowancesAsync();
```

---

## Key Business Logic

### Salary Calculation Formula
```
TotalSalary = 2,340,000 × (SalaryCof + AllowanceCof)
```
- `BASE_SALARY = 2,340,000` (constant in `SalaryCalculationService`)
- `SalaryCof` comes from the employee's current `SalaryGrade` record
- `AllowanceCof` comes from the employee's `Allowance` record (0 if no allowance)
- Result is rounded to nearest integer

### Salary Scale Resolution
When creating an employee's salary record:
1. Check `Position.SalaryScaleId` — if not null, use it
2. Otherwise, fall back to `Profession.SalaryScaleId`
3. If still null → employee gets a **fixed salary** (`FixedSalaryAmount`), no grade-based calculation

### Fixed Salary vs. Grade-Based
- **Grade-based**: `SalaryGradeId NOT NULL`, `FixedSalaryAmount NULL`
- **Fixed salary**: `SalaryGradeId NULL`, `FixedSalaryAmount NOT NULL`
- DB enforces XOR via CHECK constraint
- Fixed salary applies to top executives: Chủ tịch, Giám đốc, Phó giám đốc, Kiểm soát viên, Kế toán trưởng

### Authentication
- Password stored as SHA-512 hash
- Login: hash incoming password, compare to stored hash
- Returns `EmployeeResponse` on success, `null` on failure

### Employee Add Workflow (`AddEmployee` / `ImportEmployee`)
1. Validate: check for duplicate `IdentityCardNumber` via `GetEmployeeByIdentityCardNumber`
2. Generate new GUID for employee ID
3. Determine salary scale: `Position.SalaryScaleId ?? Profession.SalaryScaleId`
4. Create `Employee` record via `ImportEmployee(employee)`
5. If fixed salary: create `EmployeeSalary` with `FixedSalaryAmount`, `SalaryGradeId = null`
6. If grade-based: look up `SalaryGrade` by scale+level, calculate `TotalSalary`, create `EmployeeSalary`
7. Returns `true` on full success, `false` on any failure

### Employee Delete Workflow
1. Call `DeleteEmployeeSalaries(employeeId)` first (removes FK dependency)
2. Call `DeleteEmployee(employeeId)`

---

## DatabaseService Pattern

All DB service classes extend the abstract `DatabaseService` base class and call `GetSqlConnectionAsync()` per query. Pattern for every query:

```csharp
internal class FooDatabaseService : DatabaseService, IFooDBService
{
    public async Task<Foo?> GetFooByIdAsync(string id)
    {
        await using var conn = await GetSqlConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM Foo WHERE Id = @Id";
        cmd.Parameters.Add(new SqlParameter("@Id", id));

        using var reader = await cmd.ExecuteReaderAsync();
        if (reader.Read())
            return ReadFooEntity(reader);
        return null;
    }

    private Foo ReadFooEntity(SqlDataReader reader) => new Foo
    {
        Id = reader.GetString(reader.GetOrdinal("Id")),
        // ... map all columns
    };
}
```

**Rules:**
- Always use `await using` for connections and commands
- Always use parameterized queries (`@Param`)  
- Nullable columns: use `reader.IsDBNull(reader.GetOrdinal("Column")) ? null : reader.GetString(...)`
- One `ReadXxxEntity` private method per entity for consistent mapping

---

## Adding a New Feature (Pattern)

When adding a new domain (e.g., `SalaryHistory`):

1. **Model** — add `SalaryHistory.cs` to `MyHr.Data/Model/`
2. **DTO** (if needed) — add to `MyHr.Data/Dto/`
3. **Interface (DB)** — add `ISalaryHistoryDBService.cs` to `MyHr.Service/Interface/`
4. **Database service** — add `SalaryHistoryDatabaseService.cs` extending `DatabaseService`
5. **Interface (business)** — add `ISalaryHistoryService.cs` if business logic is needed
6. **Service** — add `SalaryHistoryService.cs` in `MyHr.Service/`
7. **Register DI** — add `services.AddScoped<ISalaryHistoryDBService, SalaryHistoryDatabaseService>()` in `ServiceCollectionExtension.cs`
8. **Controller** — add `SalaryHistoryController.cs` with `[Route("api/SalaryHistory")]`

When adding a new endpoint to an **existing** controller:
- Inject the needed interface via constructor
- Add the interface to `ServiceCollectionExtension.cs` if new
- Return `IActionResult`: `Ok(data)`, `NotFound(new { message })`, `BadRequest(new { message })`, `Unauthorized(new { message })`

---

## Error Response Convention

All error responses return a JSON object with a `message` field in Vietnamese:
```json
{ "message": "Không tìm thấy nhân viên" }
```
Status codes used: `200 OK`, `400 Bad Request`, `401 Unauthorized`, `404 Not Found`

---

## Deployment

- **Deployed on**: Railway (Docker)
- **Dockerfile location**: `MyHr.Api/Dockerfile` (build root = `MyHr.Api/`)
- **railway.json**: in `MyHr.Api/`, specifies `DOCKERFILE` builder
- **Container**: `mcr.microsoft.com/dotnet/aspnet:10.0` runtime image
- **Port**: reads `$PORT` env var at startup (`CMD sh -c "ASPNETCORE_URLS=http://+:${PORT} dotnet MyHr.Api.dll"`)
- **Swagger UI**: always enabled, available at `{baseUrl}/swagger`
- **Database**: Azure SQL (connection string via Railway env var `ConnectionStrings__DefaultConnection`)

### Required Railway Environment Variables
| Variable | Value |
|----------|-------|
| `ConnectionStrings__DefaultConnection` | Azure SQL ADO.NET connection string (with `Connect Timeout=60`) |
| `AllowedOrigins__0` | Frontend Vercel URL, e.g. `https://my-hr-two.vercel.app` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

### Local Development
Runs on `https://localhost:5133` (see `Properties/launchSettings.json`).
Database: LocalDB `(localdb)\MSSQLLocalDB`, database name `myhr`.
