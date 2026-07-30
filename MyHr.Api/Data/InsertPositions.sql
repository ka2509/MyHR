-- =============================================
-- MyHR - Insert Positions Data
-- AllowanceCof: Hệ số phụ cấp (tạm để 0)
-- IsManagement: 1 = Quản lý, 0 = Nhân viên
-- SalaryScaleId: Ngạch lương (chỉ áp dụng cho chức vụ quản lý)
--   - Nếu có giá trị: Dùng ngạch lương của Position
--   - Nếu NULL: Dùng ngạch lương của Profession
-- =============================================
-- LƯU Ý: Chạy script này SAU khi đã chạy InsertSalaryScales.sql
-- =============================================

-- Clear existing Position data
DELETE FROM Position;
GO

-- =============================================
-- INSERT POSITIONS
-- =============================================

-- Quản lý cấp cao (IsManagement = 1) - Không theo ngạch của Position
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-ctcty', 'CTCTY', N'Chủ tịch Công ty', 0, 1, NULL),
('pos-gd', 'GD', N'Giám đốc', 0, 1, NULL),
('pos-pgd', 'PGD', N'Phó giám đốc', 0, 1, NULL),
('pos-ksv', 'KSV', N'Kiểm soát viên', 0, 1, NULL),
('pos-ktt', 'KTT', N'Kế toán trưởng', 0, 1, NULL);

-- =============================================
-- Giám đốc Xí nghiệp → Ngạch Trưởng phòng - GĐ XN (scale-qlcc)
-- =============================================
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-gdxn', 'GDXN', N'Giám đốc Xí nghiệp', 0, 0, 'scale-qlcc');

-- =============================================
-- Phó Giám đốc Xí nghiệp → Ngạch Phó phòng - PGĐ XN (scale-qlcp)
-- =============================================
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-pgdxn', 'PGDXN', N'Phó Giám đốc Xí nghiệp', 0, 0, 'scale-qlcp');

-- =============================================
-- Trưởng phòng → Ngạch Trưởng phòng - GĐ XN (scale-qlcc)
-- =============================================
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-tp-hc', 'TPHC', N'Trưởng phòng Hành chính', 0, 0, 'scale-qlcc'),
('pos-tp-kh', 'TPKH', N'Trưởng phòng kế hoạch', 0, 0, 'scale-qlcc'),
('pos-tp-qln', 'TPQLN', N'Trưởng phòng Quản lý nước và CT', 0, 0, 'scale-qlcc'),
('pos-tp-cd', 'TPCD', N'Trưởng phòng cơ điện', 0, 0, 'scale-qlcc');

-- =============================================
-- Phó phòng → Ngạch Phó phòng - PGĐ XN (scale-qlcp)
-- =============================================
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-pp-hc', 'PPHC', N'Phó phòng Hành chính', 0, 0, 'scale-qlcp'),
('pos-pp-kh', 'PPKH', N'Phó phòng kế hoạch', 0, 0, 'scale-qlcp'),
('pos-pp-qln', 'PPQLN', N'Phó phòng quản lý nước và công trình', 0, 0, 'scale-qlcp'),
('pos-pp-cd', 'PPCD', N'Phó phòng cơ điện', 0, 0, 'scale-qlcp'),
('pos-pp', 'PP', N'Phó phòng', 0, 0, 'scale-qlcp');

-- =============================================
-- Các chức vụ KHÔNG theo ngạch của Position (SalaryScaleId = NULL)
-- Ngạch lương sẽ lấy từ Profession của nhân viên
-- =============================================

-- Cụm trưởng/phó
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-cumtruong', 'CT', N'Cụm trưởng', 0, 0, NULL),
('pos-cumpho', 'CP', N'Cụm phó', 0, 0, NULL),
('pos-congtruong', 'CGT', N'Cống trưởng', 0, 0, NULL),
('pos-tramtruong', 'TRT', N'Trạm trưởng', 0, 0, NULL);

-- Tổ trưởng
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-totruong', 'TT', N'Tổ trưởng', 0, 0, NULL),
('pos-tt-hc', 'TTHC', N'Tổ trưởng tổ hành chính', 0, 0, NULL),
('pos-tt-kt', 'TTKT', N'Tổ trưởng tổ Kế toán', 0, 0, NULL),
('pos-tt-khkt', 'TTKHKT', N'Tổ trưởng tổ kế hoạch kỹ thuật', 0, 0, NULL),
('pos-tt-kh', 'TTKH', N'Tổ trưởng tổ kế hoạch', 0, 0, NULL),
('pos-tt-sc', 'TTSC', N'Tổ trưởng tổ sửa chữa', 0, 0, NULL),
('pos-tt-sc2', 'TTSC2', N'Tổ trưởng sửa chữa', 0, 0, NULL),
('pos-tp-khkt', 'TPKHKT', N'Tổ phó tổ kế hoạch - KT', 0, 0, NULL);

-- Kế toán
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-kt', 'KT', N'Kế toán', 0, 0, NULL),
('pos-kt-ldtl', 'KTLDTL', N'Kế toán LĐTL, BHXH', 0, 0, NULL),
('pos-kt-xdcb', 'KTXDCB', N'Kế toán XDCB', 0, 0, NULL);

-- Cán bộ kế hoạch
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-cb-kh-tdct', 'CBKHTDCT', N'Cán bộ Kế hoạch TDCT', 0, 0, NULL),
('pos-cb-kh-kt', 'CBKHKT', N'Cán bộ Kế hoạch KT', 0, 0, NULL),
('pos-cb-kt-cum', 'CBKTCUM', N'Cán bộ kỹ thuật cụm', 0, 0, NULL);

-- Nhân viên
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-nv', 'NV', N'Nhân viên', 0, 0, NULL),
('pos-nv-kh', 'NVKH', N'Nhân viên Kế hoạch', 0, 0, NULL),
('pos-nv-to-khkt', 'NVTKHKT', N'Nhân viên tổ KHKT', 0, 0, NULL),
('pos-nv-to-kh', 'NVTKH', N'Nhân viên tổ Kế hoạch', 0, 0, NULL),
('pos-nv-to-khkt2', 'NVTKHKT2', N'Nhân viên tổ Kế hoạch KT', 0, 0, NULL),
('pos-nv-hc', 'NVHC', N'Nhân viên hành chính', 0, 0, NULL);

-- Quản trị/Quản lý
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-qthc', 'QTHC', N'Quản trị hành chính', 0, 0, NULL),
('pos-ql-ktct', 'QLKTCT', N'Quản lý KTCT', 0, 0, NULL);

-- Văn phòng
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-vanthu', 'VT', N'Văn thư', 0, 0, NULL),
('pos-tcvt', 'TCVT', N'Trung cấp văn thư', 0, 0, NULL),
('pos-thukho', 'TK', N'Thủ kho', 0, 0, NULL),
('pos-thuquy', 'TQ', N'Thủ quỹ', 0, 0, NULL),
('pos-thukho-quy', 'TKQ', N'Thủ kho-quỹ', 0, 0, NULL),
('pos-khoquy', 'KQ', N'Kho quỹ', 0, 0, NULL);

-- Công nhân
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-cn-qlthuynong', 'CNQLTN', N'Công nhân quản lý thủy nông', 0, 0, NULL),
('pos-cn-codien', 'CNCD', N'Công nhân cơ điện', 0, 0, NULL),
('pos-cn-handien', 'CNHD', N'Công nhân hàn điện', 0, 0, NULL),
('pos-cn-dien', 'CND', N'Công nhân điện', 0, 0, NULL),
('pos-cn-vhbd', 'CNVHBD', N'Công nhân vận hành bơm điện', 0, 0, NULL),
('pos-cn-suachua', 'CNSC', N'Công nhân sửa chữa', 0, 0, NULL),
('pos-cn-laixe', 'CNLX', N'Công nhân lái xe', 0, 0, NULL);

-- Khác
INSERT INTO Position (Id, Code, Name, AllowanceCof, IsManagement, SalaryScaleId) VALUES 
('pos-capduong', 'CD', N'Cấp dưỡng', 0, 0, NULL),
('pos-laixecon', 'LXC', N'Lái xe con', 0, 0, NULL),
('pos-laixe', 'LX', N'Lái xe', 0, 0, NULL),
('pos-baove', 'BV', N'Bảo vệ', 0, 0, NULL),
('pos-tapvu', 'TV', N'Tạp vụ', 0, 0, NULL);

GO

-- =============================================
-- Verify Data
-- =============================================
PRINT N'=== Thống kê Position ==='

SELECT 
    CASE 
        WHEN SalaryScaleId IS NOT NULL THEN N'Có ngạch lương riêng'
        ELSE N'Theo ngạch của Profession'
    END AS LoaiNgachLuong,
    COUNT(*) AS SoLuong
FROM Position
GROUP BY CASE WHEN SalaryScaleId IS NOT NULL THEN N'Có ngạch lương riêng' ELSE N'Theo ngạch của Profession' END;

SELECT 
    p.Id, 
    p.Code, 
    p.Name, 
    CASE p.IsManagement WHEN 1 THEN N'Có' ELSE N'Không' END AS QuanLy,
    ISNULL(s.Name, N'(Theo Profession)') AS NgachLuong
FROM Position p
LEFT JOIN SalaryScale s ON p.SalaryScaleId = s.Id
ORDER BY p.SalaryScaleId DESC, p.Name;

GO

DECLARE @PositionCount INT;
SELECT @PositionCount = COUNT(*) FROM Position;
PRINT N'Đã insert thành công ' + CAST(@PositionCount AS NVARCHAR) + N' Position!';
GO
