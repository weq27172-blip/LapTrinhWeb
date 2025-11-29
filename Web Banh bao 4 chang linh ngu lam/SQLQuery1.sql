USE [DBBanhBaoTuanDat]; 
GO

-- BƯỚC 1: XÓA ràng buộc khóa ngoại (DROP CONSTRAINT)
-- Sử dụng IF OBJECT_ID để đảm bảo không bị lỗi nếu ràng buộc đã bị xóa trước đó.
IF OBJECT_ID('FK_Product_Category', 'F') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Products DROP CONSTRAINT FK_Product_Category;
    PRINT 'Da xoa rang buoc FK_Product_Category.';
END
ELSE
BEGIN
    PRINT 'Khong tim thay rang buoc FK_Product_Category, bo qua viec xoa.';
END
GO

-- BƯỚC 2: CẬP NHẬT các sản phẩm cũ sang NULL (Để tránh lỗi khi TẠO LẠI khóa ngoại)
-- Thao tác này là quan trọng để cho phép tạo lại khóa ngoại ở Bước 5.
UPDATE dbo.Products
SET Category = NULL
-- WHERE Category IS NOT NULL; -- Không cần thiết nếu bạn muốn cập nhật tất cả
GO

-- BƯỚC 3: XÓA SẠCH dữ liệu cũ trong bảng Category và reset ID
TRUNCATE TABLE dbo.Categories;
GO

-- BƯỚC 4: CHÈN 3 danh mục mới vào bảng Category
INSERT INTO dbo.Categories(CodeCate, NameCate, Description)
VALUES 
    ('BB', N'Các loại bánh bao', N'Tất cả các loại bánh bao hấp, chiên'),
    ('DC', N'Các loại đồ chiên', N'Các loại chả, tôm viên, đồ ăn nhanh chiên'),
    ('CB', N'Combo đặc biệt', N'Các gói sản phẩm ưu đãi, combo');
GO

-- BƯỚC 5: TẠO LẠI Khóa ngoại đã bị xóa ở Bước 1
ALTER TABLE dbo.Products
ADD CONSTRAINT FK_Product_Category 
FOREIGN KEY (Category)
REFERENCES dbo.Categories (IDcate); 
GO

-- KIỂM TRA LẠI: Hiển thị dữ liệu mới
SELECT * FROM dbo.Categories;
GO