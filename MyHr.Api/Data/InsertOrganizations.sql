-- =============================================
-- MyHR - Insert Organizations Data
-- OrganizationType: 0=TongCongTy, 1=ChiNhanh (Xí nghiệp), 2=Phong, 3=Cum, 4=To
-- =============================================

-- Clear existing data (optional - comment out if you want to keep existing data)
DELETE FROM EmployeeSalary;
DELETE FROM Employee;
DELETE FROM Organization;
GO

-- =============================================
-- 1. CÔNG TY THỦY LỢI BẮC (Tổng công ty - Type 0)
-- =============================================
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('cty-tlb', N'Công ty Thủy lợi Bắc', 0, NULL);

-- Các phòng ban trực thuộc công ty (Type 2 - Phong)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('cty-bdh', N'Ban điều hành', 2, 'cty-tlb'),
('cty-tchc', N'Phòng Tổ chức - Hành Chính', 2, 'cty-tlb'),
('cty-taivu', N'Phòng Tài Vụ', 2, 'cty-tlb'),
('cty-khkt', N'Phòng Kế hoạch - Kỹ thuật', 2, 'cty-tlb'),
('cty-qlnct', N'Phòng Quản lý nước & công trình', 2, 'cty-tlb'),
('cty-codien', N'Phòng Cơ điện', 2, 'cty-tlb');

-- Các cụm trực thuộc công ty (Type 3 - Cum)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('cty-cum-tralinh', N'Cụm Trà Linh', 3, 'cty-tlb'),
('cty-cum-hiep', N'Cụm Hiệp', 3, 'cty-tlb'),
('cty-cum-thuyenquan', N'Cụm Thuyền Quan', 3, 'cty-tlb'),
('cty-cum-dongcong', N'Cụm Đồng Cống', 3, 'cty-tlb'),
('cty-cum-nhamlang', N'Cụm Nhâm Lang', 3, 'cty-tlb');

-- =============================================
-- 2. XÍ NGHIỆP HƯNG HÀ (Chi nhánh - Type 1)
-- =============================================
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-hungha', N'Xí nghiệp Hưng Hà', 1, 'cty-tlb');

-- Các tổ thuộc XN Hưng Hà (Type 4 - To)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-hh-to-tchc', N'Tổ Tổ chức - Hành Chính', 4, 'xn-hungha'),
('xn-hh-to-ketoan', N'Tổ Kế toán', 4, 'xn-hungha'),
('xn-hh-to-kythuat', N'Tổ Kế hoạch - Kỹ thuật', 4, 'xn-hungha'),
('xn-hh-to-cokhi', N'Tổ Cơ khí, Cơ điện', 4, 'xn-hungha');

-- Các cụm thuộc XN Hưng Hà (Type 3 - Cum)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-hh-cum-vietyen', N'Cụm Việt Yên', 3, 'xn-hungha'),
('xn-hh-cum-tramchay', N'Cụm Trạm Chay', 3, 'xn-hungha'),
('xn-hh-cum-tinhxuyen', N'Cụm Tịnh Xuyên', 3, 'xn-hungha'),
('xn-hh-cum-daothanh', N'Cụm Đào Thành', 3, 'xn-hungha'),
('xn-hh-cum-laokhe', N'Cụm Lão Khê', 3, 'xn-hungha'),
('xn-hh-cum-minhtan', N'Cụm Minh Tân', 3, 'xn-hungha');

-- =============================================
-- 3. XÍ NGHIỆP ĐÔNG HƯNG (Chi nhánh - Type 1)
-- =============================================
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-donghung', N'Xí nghiệp Đông Hưng', 1, 'cty-tlb');

-- Các tổ thuộc XN Đông Hưng (Type 4 - To)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-dh-to-tchc', N'Tổ Tổ chức - Hành Chính', 4, 'xn-donghung'),
('xn-dh-to-ketoan', N'Tổ Kế toán', 4, 'xn-donghung'),
('xn-dh-to-khkt', N'Tổ Kế hoạch - Kỹ thuật', 4, 'xn-donghung'),
('xn-dh-to-cokhicodien', N'Tổ Cơ khí, Cơ điện', 4, 'xn-donghung');

-- Các cụm thuộc XN Đông Hưng (Type 3 - Cum)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-dh-cum-hauthuong', N'Cụm Hậu Thượng', 3, 'xn-donghung'),
('xn-dh-cum-thongnhat1', N'Cụm Thống Nhất 1', 3, 'xn-donghung'),
('xn-dh-cum-quanro', N'Cụm Quán Rô', 3, 'xn-donghung'),
('xn-dh-cum-217', N'Cụm 217', 3, 'xn-donghung'),
('xn-dh-cum-thongnhat2', N'Cụm Thống Nhất 2', 3, 'xn-donghung'),
('xn-dh-cum-quanhoa', N'Cụm Quan Hoả', 3, 'xn-donghung'),
('xn-dh-cum-songhoai', N'Cụm Sông Hoài', 3, 'xn-donghung');

-- =============================================
-- 4. XÍ NGHIỆP QUỲNH PHỤ (Chi nhánh - Type 1)
-- =============================================
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-quynhphu', N'Xí nghiệp Quỳnh Phụ', 1, 'cty-tlb');

-- Các tổ thuộc XN Quỳnh Phụ (Type 4 - To)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-qp-to-tchc', N'Tổ Tổ chức - Hành Chính', 4, 'xn-quynhphu'),
('xn-qp-to-ketoan', N'Tổ Kế toán', 4, 'xn-quynhphu'),
('xn-qp-to-khkt', N'Tổ Kế hoạch - Kỹ thuật', 4, 'xn-quynhphu'),
('xn-qp-to-cokhicodien', N'Tổ Cơ khí, Cơ điện', 4, 'xn-quynhphu');

-- Các cụm thuộc XN Quỳnh Phụ (Type 3 - Cum)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-qp-cum-tanmy', N'Cụm Tân Mỹ', 3, 'xn-quynhphu'),
('xn-qp-cum-dongyenlong', N'Cụm Đông Yên Lộng', 3, 'xn-quynhphu'),
('xn-qp-cum-dainam', N'Cụm Đại Nẫm', 3, 'xn-quynhphu'),
('xn-qp-cum-caonoi', N'Cụm Cao Nội', 3, 'xn-quynhphu'),
('xn-qp-cum-tayyenlong', N'Cụm Tây Yên Lộng', 3, 'xn-quynhphu'),
('xn-qp-cum-neo', N'Cụm Neo', 3, 'xn-quynhphu'),
('xn-qp-cum-trangxa', N'Cụm Trang Xá', 3, 'xn-quynhphu'),
('xn-qp-cum-quynhhoa', N'Cụm Quỳnh Hoa', 3, 'xn-quynhphu');

-- =============================================
-- 5. XÍ NGHIỆP THÁI THỤY (Chi nhánh - Type 1)
-- =============================================
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-thaithuy', N'Xí nghiệp Thái Thụy', 1, 'cty-tlb');

-- Các tổ thuộc XN Thái Thụy (Type 4 - To)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-tt-to-tchc', N'Tổ Tổ chức - Hành Chính', 4, 'xn-thaithuy'),
('xn-tt-to-ketoan', N'Tổ Kế toán', 4, 'xn-thaithuy'),
('xn-tt-to-khkt', N'Tổ Kế hoạch - Kỹ thuật', 4, 'xn-thaithuy'),
('xn-tt-to-cokhicodien', N'Tổ Cơ khí, Cơ điện', 4, 'xn-thaithuy');

-- Các cụm thuộc XN Thái Thụy (Type 3 - Cum)
INSERT INTO Organization (Id, Name, Type, ParentId) VALUES 
('xn-tt-cum-thaiphuc', N'Cụm Thái Phúc', 3, 'xn-thaithuy'),
('xn-tt-cum-thaihong', N'Cụm Thái Hồng', 3, 'xn-thaithuy'),
('xn-tt-cum-tnhe', N'Cụm TN Hệ', 3, 'xn-thaithuy'),
('xn-tt-cum-thaihoc', N'Cụm Thái Học', 3, 'xn-thaithuy'),
('xn-tt-cum-tnthaido', N'Cụm TN Thái Đô', 3, 'xn-thaithuy'),
('xn-tt-cum-phonglam', N'Cụm Phong Lẫm', 3, 'xn-thaithuy'),
('xn-tt-cum-tnthuyyquynh', N'Cụm TN Thụy Quỳnh', 3, 'xn-thaithuy'),
('xn-tt-cum-tntruongxuanha', N'Cụm TN Trường Xuân Hà', 3, 'xn-thaithuy');

GO

-- =============================================
-- Verify Data
-- =============================================
PRINT N'=== Thống kê tổ chức ==='

SELECT 
    CASE Type 
        WHEN 0 THEN N'Tổng công ty'
        WHEN 1 THEN N'Xí nghiệp'
        WHEN 2 THEN N'Phòng'
        WHEN 3 THEN N'Cụm'
        WHEN 4 THEN N'Tổ'
    END AS LoaiToChuc,
    COUNT(*) AS SoLuong
FROM Organization
GROUP BY Type
ORDER BY Type;

SELECT 
    o1.Name AS TenToChuc,
    CASE o1.Type 
        WHEN 0 THEN N'Tổng công ty'
        WHEN 1 THEN N'Xí nghiệp'
        WHEN 2 THEN N'Phòng'
        WHEN 3 THEN N'Cụm'
        WHEN 4 THEN N'Tổ'
    END AS LoaiToChuc,
    o2.Name AS TrucThuoc
FROM Organization o1
LEFT JOIN Organization o2 ON o1.ParentId = o2.Id
ORDER BY o1.Type, o2.Name, o1.Name;

GO

PRINT N'Đã insert thành công dữ liệu Organization!';
GO
