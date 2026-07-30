-- =============================================
-- MyHR - Insert Professions Data (Trình độ/Bằng cấp)
-- SalaryScaleId: Ngạch lương tương ứng
--   - NULL: Chưa xác định hoặc theo Position
--   - scale-cnks: Cử nhân - Kỹ sư
--   - scale-csktv: Cán sự - Kỹ thuật viên
--   - scale-cn1: Công nhân nhóm I
--   - scale-cn2: Công nhân nhóm II
--   - scale-lx: Lái xe
-- =============================================
-- LƯU Ý: Chạy script này SAU khi đã chạy InsertSalaryScales.sql
-- =============================================

-- Clear existing Profession data
DELETE FROM Profession;
GO

-- =============================================
-- INSERT PROFESSIONS
-- =============================================

-- Thạc sĩ / Đại học (NULL - quản lý cấp cao, lương cố định)
INSERT INTO Profession (Id, Code, Name, SalaryScaleId) VALUES 
('prof-thacsi', 'THS', N'Thạc sĩ', NULL),
('prof-dhbk', 'DHBK', N'Đại học Bách khoa', NULL);

-- Cử nhân / Kỹ sư → Ngạch CNKS (Cử nhân - Kỹ sư)
INSERT INTO Profession (Id, Code, Name, SalaryScaleId) VALUES 
('prof-cnkt', 'CNKT', N'Cử nhân kinh tế', 'scale-cnks'),
('prof-kstl', 'KSTL', N'Kỹ sư thủy lợi', 'scale-cnks'),
('prof-ksqldd', 'KSQLDD', N'Kỹ sư quản lý đất đai', 'scale-cnks'),
('prof-ksgtvt', 'KSGTVT', N'Kỹ sư Giao thông vận tải', 'scale-cnks'),
('prof-ksdien', 'KSD', N'Kỹ sư điện', 'scale-cnks'),
('prof-ksxd', 'KSXD', N'Kỹ sư xây dựng', 'scale-cnks'),
('prof-cnqttc', 'CNQTTC', N'Cử nhân quản trị tài chính', 'scale-cnks'),
('prof-cnkt2', 'CNKT2', N'Cử nhân kế toán', 'scale-cnks');

-- Cao đẳng / Trung cấp → Ngạch CSKTV (Cán sự - Kỹ thuật viên)
INSERT INTO Profession (Id, Code, Name, SalaryScaleId) VALUES 
('prof-cdkt', 'CDKT', N'Cao đẳng kế toán', 'scale-csktv'),
('prof-tcdien', 'TCD', N'Trung cấp điện', 'scale-csktv'),
('prof-tckt', 'TCKT', N'Trung cấp kế toán', 'scale-csktv'),
('prof-cdtl', 'CDTL', N'Cao đẳng thủy lợi', 'scale-csktv'),
('prof-tctl', 'TCTL', N'Trung cấp thủy lợi', 'scale-csktv'),
('prof-tcvt', 'TCVT', N'Trung cấp văn thư', 'scale-csktv'),
('prof-cddien', 'CDD', N'Cao đẳng điện', 'scale-csktv');

-- Công nhân nhóm I → Ngạch CN1 (Quản lý thủy nông, vận hành bơm < 8000 m³/h)
INSERT INTO Profession (Id, Code, Name, SalaryScaleId) VALUES 
('prof-cnqltn', 'CNQLTN', N'Công nhân quản lý thủy nông', 'scale-cn1');

-- Công nhân nhóm II → Ngạch CN2 (Sửa chữa cơ khí cơ điện, vận hành bơm > 8000 m³/h)
INSERT INTO Profession (Id, Code, Name, SalaryScaleId) VALUES 
('prof-cncodien', 'CNCD', N'Công nhân cơ điện', 'scale-cn2'),
('prof-cnhandien', 'CNHD', N'Công nhân hàn điện', 'scale-cn2'),
('prof-cnscckcd', 'CNSCCK', N'Công nhân sửa chữa cơ khí cơ điện', 'scale-cn2'),
('prof-cndien', 'CND', N'Công nhân điện', 'scale-cn2'),
('prof-cnvhbd', 'CNVHBD', N'Công nhân vận hành bơm điện', 'scale-cn2'),
('prof-cnsc', 'CNSC', N'Công nhân sửa chữa', 'scale-cn2'),
('prof-cnckcd', 'CNCKCD', N'Công nhân cơ khí, cơ điện', 'scale-cn2');

-- Lái xe → Ngạch LX
INSERT INTO Profession (Id, Code, Name, SalaryScaleId) VALUES 
('prof-cnlx', 'CNLX', N'Công nhân lái xe', 'scale-lx'),
('prof-lxcon', 'LXC', N'Lái xe con', 'scale-lx');

GO

-- =============================================
-- Verify Data
-- =============================================
PRINT N'=== Thống kê Profession theo Ngạch lương ==='

SELECT 
    ISNULL(s.Name, N'(Chưa xác định)') AS NgachLuong,
    COUNT(*) AS SoLuong
FROM Profession p
LEFT JOIN SalaryScale s ON p.SalaryScaleId = s.Id
GROUP BY s.Name
ORDER BY SoLuong DESC;

SELECT 
    p.Id,
    p.Code,
    p.Name AS TrinhDo,
    ISNULL(s.Name, N'(Chưa xác định)') AS NgachLuong,
    ISNULL(CAST(s.MaxGrade AS NVARCHAR), N'-') AS BacMax
FROM Profession p
LEFT JOIN SalaryScale s ON p.SalaryScaleId = s.Id
ORDER BY s.Name, p.Name;

GO

DECLARE @ProfessionCount INT;
SELECT @ProfessionCount = COUNT(*) FROM Profession;
PRINT N'Đã insert thành công ' + CAST(@ProfessionCount AS NVARCHAR) + N' Profession!';
GO
