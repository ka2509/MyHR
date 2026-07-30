-- =============================================
-- MyHR - Insert SalaryGrades Data (Bậc lương)
-- Dữ liệu thực tế với hệ số điều chỉnh 0.3
-- =============================================
-- LƯU Ý: Chạy script này SAU khi đã chạy InsertSalaryScales.sql
-- =============================================

-- Clear existing SalaryGrade data
DELETE FROM SalaryGrade;
GO

-- =============================================
-- 1. NGẠCH TRƯỞNG PHÒNG - GĐ XN (scale-qlcc)
-- 6 bậc, 36 tháng (3 năm) nhảy bậc
-- Hệ số điều chỉnh: 0.3
-- =============================================
INSERT INTO SalaryGrade (Id, SalaryScaleId, GradeLevel, SalaryCof, BaseSalary, PromotionMonthsMale, PromotionMonthsFemale) VALUES 
('sg-qlcc-1', 'scale-qlcc', 1, 4.00, 13689000, 36, 36),
('sg-qlcc-2', 'scale-qlcc', 2, 4.33, 14692860, 36, 36),
('sg-qlcc-3', 'scale-qlcc', 3, 4.66, 15696720, 36, 36),
('sg-qlcc-4', 'scale-qlcc', 4, 4.99, 16700580, 36, 36),
('sg-qlcc-5', 'scale-qlcc', 5, 5.32, 17704440, 36, 36),
('sg-qlcc-6', 'scale-qlcc', 6, 5.65, 18708300, 0, 0);  -- Bậc cuối

-- =============================================
-- 2. NGẠCH PHÓ PHÒNG - PGĐ XN (scale-qlcp)
-- 6 bậc, 36 tháng (3 năm) nhảy bậc
-- Hệ số điều chỉnh: 0.3
-- Chung hệ số với Trưởng phòng, khác mức lương cơ bản
-- =============================================
INSERT INTO SalaryGrade (Id, SalaryScaleId, GradeLevel, SalaryCof, BaseSalary, PromotionMonthsMale, PromotionMonthsFemale) VALUES 
('sg-qlcp-1', 'scale-qlcp', 1, 4.00, 12168000, 36, 36),
('sg-qlcp-2', 'scale-qlcp', 2, 4.33, 13171860, 36, 36),
('sg-qlcp-3', 'scale-qlcp', 3, 4.66, 14175720, 36, 36),
('sg-qlcp-4', 'scale-qlcp', 4, 4.99, 15179580, 36, 36),
('sg-qlcp-5', 'scale-qlcp', 5, 5.32, 16183440, 36, 36),
('sg-qlcp-6', 'scale-qlcp', 6, 5.65, 17187300, 0, 0);  -- Bậc cuối

-- =============================================
-- 3. NGẠCH CỬ NHÂN KINH TẾ - KỸ SƯ (scale-cnks)
-- 8 bậc, 36 tháng (3 năm) nhảy bậc
-- Hệ số điều chỉnh: 0.3
-- =============================================
INSERT INTO SalaryGrade (Id, SalaryScaleId, GradeLevel, SalaryCof, BaseSalary, PromotionMonthsMale, PromotionMonthsFemale) VALUES 
('sg-cnks-1', 'scale-cnks', 1, 2.34, 7118280, 36, 36),
('sg-cnks-2', 'scale-cnks', 2, 2.65, 8061300, 36, 36),
('sg-cnks-3', 'scale-cnks', 3, 2.96, 9004320, 36, 36),
('sg-cnks-4', 'scale-cnks', 4, 3.27, 9947340, 36, 36),
('sg-cnks-5', 'scale-cnks', 5, 3.58, 10890360, 36, 36),
('sg-cnks-6', 'scale-cnks', 6, 3.89, 11833380, 36, 36),
('sg-cnks-7', 'scale-cnks', 7, 4.20, 12776400, 36, 36),
('sg-cnks-8', 'scale-cnks', 8, 4.51, 13719420, 0, 0);  -- Bậc cuối

-- =============================================
-- 4. NGẠCH CÁN SỰ - KỸ THUẬT VIÊN (scale-csktv)
-- 12 bậc, 24 tháng (2 năm) nhảy bậc
-- Hệ số điều chỉnh: 0.3
-- =============================================
INSERT INTO SalaryGrade (Id, SalaryScaleId, GradeLevel, SalaryCof, BaseSalary, PromotionMonthsMale, PromotionMonthsFemale) VALUES 
('sg-csktv-1', 'scale-csktv', 1, 1.80, 5475600, 24, 24),
('sg-csktv-2', 'scale-csktv', 2, 1.99, 6053580, 24, 24),
('sg-csktv-3', 'scale-csktv', 3, 2.18, 6631560, 24, 24),
('sg-csktv-4', 'scale-csktv', 4, 2.37, 7209540, 24, 24),
('sg-csktv-5', 'scale-csktv', 5, 2.56, 7787520, 24, 24),
('sg-csktv-6', 'scale-csktv', 6, 2.75, 8365500, 24, 24),
('sg-csktv-7', 'scale-csktv', 7, 2.94, 8943480, 24, 24),
('sg-csktv-8', 'scale-csktv', 8, 3.13, 9521460, 24, 24),
('sg-csktv-9', 'scale-csktv', 9, 3.32, 10099440, 24, 24),
('sg-csktv-10', 'scale-csktv', 10, 3.51, 10677420, 24, 24),
('sg-csktv-11', 'scale-csktv', 11, 3.70, 11255400, 24, 24),
('sg-csktv-12', 'scale-csktv', 12, 3.89, 11833380, 0, 0);  -- Bậc cuối

-- =============================================
-- 5. NGẠCH NHÂN VIÊN VĂN THƯ (scale-nvvt)
-- 12 bậc, 24 tháng (2 năm) nhảy bậc
-- Hệ số điều chỉnh: 0.3
-- =============================================
INSERT INTO SalaryGrade (Id, SalaryScaleId, GradeLevel, SalaryCof, BaseSalary, PromotionMonthsMale, PromotionMonthsFemale) VALUES 
('sg-nvvt-1', 'scale-nvvt', 1, 1.35, 4106700, 24, 24),
('sg-nvvt-2', 'scale-nvvt', 2, 1.53, 4654260, 24, 24),
('sg-nvvt-3', 'scale-nvvt', 3, 1.71, 5201820, 24, 24),
('sg-nvvt-4', 'scale-nvvt', 4, 1.89, 5749380, 24, 24),
('sg-nvvt-5', 'scale-nvvt', 5, 2.07, 6296940, 24, 24),
('sg-nvvt-6', 'scale-nvvt', 6, 2.25, 6844500, 24, 24),
('sg-nvvt-7', 'scale-nvvt', 7, 2.43, 7392060, 24, 24),
('sg-nvvt-8', 'scale-nvvt', 8, 2.61, 7939620, 24, 24),
('sg-nvvt-9', 'scale-nvvt', 9, 2.79, 8487180, 24, 24),
('sg-nvvt-10', 'scale-nvvt', 10, 2.97, 9034740, 24, 24),
('sg-nvvt-11', 'scale-nvvt', 11, 3.15, 9582300, 24, 24),
('sg-nvvt-12', 'scale-nvvt', 12, 3.33, 10129860, 0, 0);  -- Bậc cuối

-- =============================================
-- 6. NGẠCH NHÂN VIÊN PHỤC VỤ (scale-nvpv)
-- 12 bậc, 24 tháng (2 năm) nhảy bậc
-- Hệ số điều chỉnh: 0.3
-- =============================================
INSERT INTO SalaryGrade (Id, SalaryScaleId, GradeLevel, SalaryCof, BaseSalary, PromotionMonthsMale, PromotionMonthsFemale) VALUES 
('sg-nvpv-1', 'scale-nvpv', 1, 1.00, 3042000, 24, 24),
('sg-nvpv-2', 'scale-nvpv', 2, 1.18, 3589560, 24, 24),
('sg-nvpv-3', 'scale-nvpv', 3, 1.36, 4137120, 24, 24),
('sg-nvpv-4', 'scale-nvpv', 4, 1.54, 4684680, 24, 24),
('sg-nvpv-5', 'scale-nvpv', 5, 1.72, 5232240, 24, 24),
('sg-nvpv-6', 'scale-nvpv', 6, 1.90, 5779800, 24, 24),
('sg-nvpv-7', 'scale-nvpv', 7, 2.08, 6327360, 24, 24),
('sg-nvpv-8', 'scale-nvpv', 8, 2.26, 6874920, 24, 24),
('sg-nvpv-9', 'scale-nvpv', 9, 2.44, 7422480, 24, 24),
('sg-nvpv-10', 'scale-nvpv', 10, 2.62, 7970040, 24, 24),
('sg-nvpv-11', 'scale-nvpv', 11, 2.80, 8517600, 24, 24),
('sg-nvpv-12', 'scale-nvpv', 12, 2.98, 9065160, 0, 0);  -- Bậc cuối

-- =============================================
-- 7. NGẠCH CÔNG NHÂN NHÓM I (scale-cn1)
-- Quản lý thủy nông, VHBĐ < 8000 m3
-- 7 bậc, thời gian nhảy bậc khác nhau theo giới tính
-- Hệ số điều chỉnh: 0.3
-- =============================================
INSERT INTO SalaryGrade (Id, SalaryScaleId, GradeLevel, SalaryCof, BaseSalary, PromotionMonthsMale, PromotionMonthsFemale) VALUES 
('sg-cn1-1', 'scale-cn1', 1, 1.55, 4715100, 24, 24),   -- 1→2: 2 năm
('sg-cn1-2', 'scale-cn1', 2, 1.83, 5566860, 24, 24),   -- 2→3: 2 năm
('sg-cn1-3', 'scale-cn1', 3, 2.16, 6570720, 48, 36),   -- 3→4: Nam 4 năm, Nữ 3 năm
('sg-cn1-4', 'scale-cn1', 4, 2.55, 7757100, 60, 48),   -- 4→5: Nam 5 năm, Nữ 4 năm
('sg-cn1-5', 'scale-cn1', 5, 3.01, 9156420, 72, 60),   -- 5→6: Nam 6 năm, Nữ 5 năm
('sg-cn1-6', 'scale-cn1', 6, 3.56, 10829520, 84, 84),  -- 6→7: 7 năm
('sg-cn1-7', 'scale-cn1', 7, 4.20, 12776400, 0, 0);    -- Bậc cuối

-- =============================================
-- 8. NGẠCH CÔNG NHÂN NHÓM II (scale-cn2)
-- Sửa chữa cơ khí cơ điện, VHBĐ > 8000 m3
-- 7 bậc, thời gian nhảy bậc khác nhau theo giới tính
-- Hệ số điều chỉnh: 0.3
-- =============================================
INSERT INTO SalaryGrade (Id, SalaryScaleId, GradeLevel, SalaryCof, BaseSalary, PromotionMonthsMale, PromotionMonthsFemale) VALUES 
('sg-cn2-1', 'scale-cn2', 1, 1.67, 5080140, 24, 24),   -- 1→2: 2 năm
('sg-cn2-2', 'scale-cn2', 2, 1.96, 5962320, 24, 24),   -- 2→3: 2 năm
('sg-cn2-3', 'scale-cn2', 3, 2.31, 7027020, 48, 36),   -- 3→4: Nam 4 năm, Nữ 3 năm
('sg-cn2-4', 'scale-cn2', 4, 2.71, 8243820, 60, 48),   -- 4→5: Nam 5 năm, Nữ 4 năm
('sg-cn2-5', 'scale-cn2', 5, 3.19, 9703980, 72, 60),   -- 5→6: Nam 6 năm, Nữ 5 năm
('sg-cn2-6', 'scale-cn2', 6, 3.74, 11377080, 84, 84),  -- 6→7: 7 năm
('sg-cn2-7', 'scale-cn2', 7, 4.40, 13384800, 0, 0);    -- Bậc cuối

-- =============================================
-- 9. NGẠCH LÁI XE (scale-lx)
-- 4 bậc, thời gian nhảy bậc tăng dần
-- Hệ số điều chỉnh: 0.3
-- =============================================
INSERT INTO SalaryGrade (Id, SalaryScaleId, GradeLevel, SalaryCof, BaseSalary, PromotionMonthsMale, PromotionMonthsFemale) VALUES 
('sg-lx-1', 'scale-lx', 1, 2.18, 6631560, 36, 36),     -- 1→2: 3 năm
('sg-lx-2', 'scale-lx', 2, 2.57, 7817940, 72, 72),     -- 2→3: 6 năm
('sg-lx-3', 'scale-lx', 3, 3.05, 9278100, 108, 108),   -- 3→4: 9 năm
('sg-lx-4', 'scale-lx', 4, 3.60, 10951200, 0, 0);      -- Bậc cuối

GO

-- =============================================
-- Verify Data
-- =============================================
PRINT N'=== Thống kê SalaryGrade theo Ngạch lương ==='

SELECT 
    s.Name AS NgachLuong,
    COUNT(g.Id) AS SoBac,
    MIN(g.SalaryCof) AS HeSoMin,
    MAX(g.SalaryCof) AS HeSoMax,
    FORMAT(MIN(g.BaseSalary), 'N0') AS LuongMin,
    FORMAT(MAX(g.BaseSalary), 'N0') AS LuongMax
FROM SalaryScale s
LEFT JOIN SalaryGrade g ON s.Id = g.SalaryScaleId
GROUP BY s.Id, s.Name
ORDER BY MIN(g.BaseSalary) DESC;

-- Chi tiết các ngạch có thời gian nhảy bậc khác nhau theo giới tính
PRINT N''
PRINT N'=== Ngạch CN1, CN2 - Thời gian nhảy bậc theo giới tính ==='

SELECT 
    s.Name AS NgachLuong,
    g.GradeLevel AS Bac,
    g.SalaryCof AS HeSo,
    FORMAT(g.BaseSalary, 'N0') AS MucLuong,
    g.PromotionMonthsMale AS ThoiGianNam,
    g.PromotionMonthsFemale AS ThoiGianNu,
    CASE 
        WHEN g.PromotionMonthsMale <> g.PromotionMonthsFemale THEN N'⚡ Khác nhau'
        ELSE N''
    END AS GhiChu
FROM SalaryGrade g
JOIN SalaryScale s ON g.SalaryScaleId = s.Id
WHERE s.Code IN ('CN1', 'CN2')
ORDER BY s.Name, g.GradeLevel;

GO

DECLARE @GradeCount INT;
SELECT @GradeCount = COUNT(*) FROM SalaryGrade;
PRINT N'Đã insert thành công ' + CAST(@GradeCount AS NVARCHAR) + N' SalaryGrade!';
GO
