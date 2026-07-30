-- =============================================
-- Insert SalaryScale (Ngạch lương)
-- =============================================

-- Xóa dữ liệu cũ (nếu có)
DELETE FROM SalaryGrade;
DELETE FROM Profession;
DELETE FROM SalaryScale;
GO

-- Insert các ngạch lương
-- Ngạch quản lý
INSERT INTO SalaryScale (Id, Code, Name, MaxGrade) 
VALUES ('scale-qlcc', 'QLCC', N'Trưởng phòng - Giám đốc XN', 6);

INSERT INTO SalaryScale (Id, Code, Name, MaxGrade) 
VALUES ('scale-qlcp', 'QLCP', N'Phó phòng - Phó Giám đốc XN', 6);

-- Ngạch chuyên môn
INSERT INTO SalaryScale (Id, Code, Name, MaxGrade) 
VALUES ('scale-cnks', 'CNKS', N'Cử nhân - Kỹ sư', 8);

INSERT INTO SalaryScale (Id, Code, Name, MaxGrade) 
VALUES ('scale-csktv', 'CSKTV', N'Cán sự - Kỹ thuật viên', 12);

INSERT INTO SalaryScale (Id, Code, Name, MaxGrade) 
VALUES ('scale-nvvt', 'NVVT', N'Nhân viên văn thư', 12);

INSERT INTO SalaryScale (Id, Code, Name, MaxGrade) 
VALUES ('scale-nvpv', 'NVPV', N'Nhân viên phục vụ', 12);

-- Ngạch công nhân
INSERT INTO SalaryScale (Id, Code, Name, MaxGrade) 
VALUES ('scale-cn1', 'CN1', N'Công nhân nhóm I', 7);
-- Ghi chú: Quản lý thủy nông, vận hành bơm điện < 8000 m3/h

INSERT INTO SalaryScale (Id, Code, Name, MaxGrade) 
VALUES ('scale-cn2', 'CN2', N'Công nhân nhóm II', 7);
-- Ghi chú: Sửa chữa cơ khí cơ điện, vận hành bơm điện > 8000 m3/h

INSERT INTO SalaryScale (Id, Code, Name, MaxGrade) 
VALUES ('scale-lx', 'LX', N'Lái xe', 4);

GO

-- Kiểm tra kết quả
SELECT * FROM SalaryScale ORDER BY MaxGrade DESC, Name;
GO

PRINT 'Insert SalaryScale completed successfully!';
GO
