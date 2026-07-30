-- =============================================
-- MyHR Database Creation Script
-- =============================================

-- =============================================
-- Drop existing tables (nếu cần recreate)
-- =============================================
IF OBJECT_ID('dbo.EmployeeSalary', 'U') IS NOT NULL DROP TABLE dbo.EmployeeSalary;
IF OBJECT_ID('dbo.Employee', 'U') IS NOT NULL DROP TABLE dbo.Employee;
IF OBJECT_ID('dbo.Allowance', 'U') IS NOT NULL DROP TABLE dbo.Allowance;
IF OBJECT_ID('dbo.SalaryGrade', 'U') IS NOT NULL DROP TABLE dbo.SalaryGrade;
IF OBJECT_ID('dbo.Profession', 'U') IS NOT NULL DROP TABLE dbo.Profession;
IF OBJECT_ID('dbo.Position', 'U') IS NOT NULL DROP TABLE dbo.Position;
IF OBJECT_ID('dbo.SalaryScale', 'U') IS NOT NULL DROP TABLE dbo.SalaryScale;
IF OBJECT_ID('dbo.Organization', 'U') IS NOT NULL DROP TABLE dbo.Organization;
GO

-- =============================================
-- 1. Organization (Tổ chức/Phòng ban)
-- OrganizationType: 0=TongCongTy, 1=ChiNhanh, 2=Phong, 3=Cum, 4=To
-- =============================================
CREATE TABLE Organization (
    Id NVARCHAR(50) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Type INT NOT NULL,
    ParentId NVARCHAR(50) NULL,
    CONSTRAINT FK_Organization_Parent FOREIGN KEY (ParentId) REFERENCES Organization(Id)
);
GO

-- =============================================
-- 2. SalaryScale (Ngạch lương)
-- Nhiều trình độ (Profession) có thể cùng thuộc một ngạch lương
-- =============================================
CREATE TABLE SalaryScale (
    Id NVARCHAR(50) PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    MaxGrade INT NOT NULL
);
GO

-- =============================================
-- 3. Position (Chức vụ/Chuyên môn nghiệp vụ)
-- SalaryScaleId: Chỉ các chức vụ quản lý mới có giá trị
-- Nếu NULL, ngạch lương sẽ lấy từ Profession của nhân viên
-- =============================================
CREATE TABLE Position (
    Id NVARCHAR(50) PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    AllowanceCof DECIMAL(10, 2) NOT NULL DEFAULT 0,
    IsManagement BIT NOT NULL DEFAULT 0,
    SalaryScaleId NVARCHAR(50) NULL,
    CONSTRAINT FK_Position_SalaryScale FOREIGN KEY (SalaryScaleId) REFERENCES SalaryScale(Id)
);
GO

-- =============================================
-- 4. Profession (Trình độ/Bằng cấp)
-- Ví dụ: Công nhân hàn điện, Công nhân cơ điện đều thuộc Ngạch Công nhân
-- ============================================= 
CREATE TABLE Profession (
    Id NVARCHAR(50) PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    SalaryScaleId NVARCHAR(50) NULL,
    CONSTRAINT FK_Profession_SalaryScale FOREIGN KEY (SalaryScaleId) REFERENCES SalaryScale(Id)
);
GO

-- =============================================
-- 5. SalaryGrade (Bậc lương)
-- Liên kết với SalaryScale (ngạch lương), không phải Profession
-- PromotionMonthsMale/Female: Số tháng để tăng lên bậc tiếp theo (có thể khác nhau theo giới tính)
-- =============================================
CREATE TABLE SalaryGrade (
    Id NVARCHAR(50) PRIMARY KEY,
    SalaryScaleId NVARCHAR(50) NOT NULL,
    GradeLevel INT NOT NULL,
    SalaryCof DECIMAL(10, 2) NOT NULL,
    BaseSalary DECIMAL(18, 0) NOT NULL,
    PromotionMonthsMale INT NOT NULL DEFAULT 24,    -- Thời gian nhảy bậc cho Nam
    PromotionMonthsFemale INT NOT NULL DEFAULT 24,  -- Thời gian nhảy bậc cho Nữ
    CONSTRAINT FK_SalaryGrade_SalaryScale FOREIGN KEY (SalaryScaleId) REFERENCES SalaryScale(Id),
    CONSTRAINT UQ_SalaryGrade_ScaleLevel UNIQUE (SalaryScaleId, GradeLevel)
);
GO

-- =============================================
-- 6. Allowance (Phụ cấp)
-- Type: 1=Responsibility (Trách nhiệm), 2=Job (Công việc)
-- Each employee can have at most one allowance
-- =============================================
CREATE TABLE Allowance (
    Id NVARCHAR(50) PRIMARY KEY,
    Type INT NOT NULL,
    Level INT NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Coefficient DECIMAL(10, 2) NOT NULL,
    CONSTRAINT UQ_Allowance_TypeLevel UNIQUE (Type, Level),
    CONSTRAINT CK_Allowance_Type CHECK (Type IN (1, 2)),
    CONSTRAINT CK_Allowance_Level CHECK (Level > 0)
);
GO

-- =============================================
-- 7. Employee (Nhân viên)
-- Sex: 0=Female, 1=Male
-- AllowanceId: Optional - employee can have one allowance or none
-- =============================================
CREATE TABLE Employee (
    Id NVARCHAR(50) PRIMARY KEY,
    OrganizationId NVARCHAR(50) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Sex INT NOT NULL,
    SocialInsurance NVARCHAR(50) NOT NULL,
    Dob DATE NOT NULL,
    IdentityCardNumber NVARCHAR(20) NOT NULL,
    SocialInsuranceContributionDate DATE NOT NULL,
    PositionId NVARCHAR(50) NOT NULL,
    ProfessionId NVARCHAR(50) NOT NULL,
    Password NVARCHAR(500) NOT NULL,
    AllowanceId NVARCHAR(50) NULL,
    CONSTRAINT FK_Employee_Organization FOREIGN KEY (OrganizationId) REFERENCES Organization(Id),
    CONSTRAINT FK_Employee_Position FOREIGN KEY (PositionId) REFERENCES Position(Id),
    CONSTRAINT FK_Employee_Profession FOREIGN KEY (ProfessionId) REFERENCES Profession(Id),
    CONSTRAINT FK_Employee_Allowance FOREIGN KEY (AllowanceId) REFERENCES Allowance(Id),
    CONSTRAINT UQ_Employee_IdentityCardNumber UNIQUE (IdentityCardNumber),
    CONSTRAINT UQ_Employee_SocialInsurance UNIQUE (SocialInsurance)
);
GO

-- =============================================
-- 8. EmployeeSalary (Lịch sử lương nhân viên)
-- SalaryGradeId: NULL for fixed salary employees (Ban điều hành)
-- FixedSalaryAmount: Only used when SalaryGradeId IS NULL
-- TotalSalary: Final salary amount (calculated or fixed)
-- =============================================
CREATE TABLE EmployeeSalary (
    Id NVARCHAR(50) PRIMARY KEY,
    EmployeeId NVARCHAR(50) NOT NULL,
    SalaryGradeId NVARCHAR(50) NULL,  -- Nullable for executives with fixed salary
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,
    Reason NVARCHAR(500) NULL,
    FixedSalaryAmount DECIMAL(18, 0) NULL,  -- Only for executives (when SalaryGradeId IS NULL)
    TotalSalary DECIMAL(18, 0) NOT NULL DEFAULT 0,
    CONSTRAINT FK_EmployeeSalary_Employee FOREIGN KEY (EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeSalary_SalaryGrade FOREIGN KEY (SalaryGradeId) REFERENCES SalaryGrade(Id),
    CONSTRAINT CK_EmployeeSalary_SalaryType CHECK (
        (SalaryGradeId IS NOT NULL AND FixedSalaryAmount IS NULL) OR  -- Calculated salary
        (SalaryGradeId IS NULL AND FixedSalaryAmount IS NOT NULL)      -- Fixed salary
    )
);
GO

-- Create index for common queries
CREATE INDEX IX_Employee_IdentityCardNumber ON Employee(IdentityCardNumber);
CREATE INDEX IX_EmployeeSalary_EmployeeId ON EmployeeSalary(EmployeeId);
CREATE INDEX IX_EmployeeSalary_CurrentSalary ON EmployeeSalary(EmployeeId, EffectiveTo) WHERE EffectiveTo IS NULL;
CREATE INDEX IX_Profession_SalaryScaleId ON Profession(SalaryScaleId);
CREATE INDEX IX_Position_SalaryScaleId ON Position(SalaryScaleId);
GO

PRINT 'Tables created successfully!';
GO
