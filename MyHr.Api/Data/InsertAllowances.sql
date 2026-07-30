-- =============================================
-- Insert Allowances (Phụ cấp)
-- =============================================

PRINT 'Inserting Allowances...';

-- Delete existing allowances to avoid duplicates
DELETE FROM Allowance;

-- Insert Allowance data
-- Type: 1 = Responsibility Allowance (Phụ cấp trách nhiệm)
-- Type: 2 = Job Allowance (Phụ cấp công việc)
INSERT INTO Allowance (Id, Type, Level, Name, Coefficient) VALUES
-- Responsibility Allowance - 2 levels
('allow-resp-1', 1, 1, N'Phụ cấp trách nhiệm - Mức 1', 0.5),
('allow-resp-2', 1, 2, N'Phụ cấp trách nhiệm - Mức 2', 0.3),

-- Job Allowance - 2 levels
('allow-job-1', 2, 1, N'Phụ cấp công việc - Mức 1', 0.2),
('allow-job-2', 2, 2, N'Phụ cấp công việc - Mức 2', 0.1);

SELECT 
    Id,
    CASE Type
        WHEN 1 THEN N'Trách nhiệm'
        WHEN 2 THEN N'Công việc'
    END AS AllowanceType,
    Level,
    Name,
    Coefficient
FROM Allowance
ORDER BY Type, Level;

PRINT 'Inserted 4 allowances successfully!';
GO
