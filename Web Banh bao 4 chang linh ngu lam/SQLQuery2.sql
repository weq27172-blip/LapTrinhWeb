ALTER TABLE dbo.Customers
ADD PassCus NVARCHAR(50) NULL; -- Cho phép NULL tạm thời để không lỗi dữ liệu cũ
GO

-- Cập nhật mật khẩu mặc định cho các khách hàng cũ (nếu có) là '123'
UPDATE dbo.Customers
SET PassCus = '123'
WHERE PassCus IS NULL;
GO