# FitZone — Quản lý hội viên

1. Chạy script Database/FitZone.sql trong SQL Server Management Studio trên server muốn sử dụng.
2. Mặc định ứng dụng kết nối localhost, database FitZone bằng Windows Authentication. Nếu dùng server khác, đặt biến môi trường FITZONE_CONNECTION_STRING hoặc sửa chuỗi kết nối trong Cau2/Data/FitZoneContext.cs.
3. Mở Cau2.slnx bằng Visual Studio có .NET 10 SDK, restore NuGet và chạy dự án Cau2.

## Database First với EF Core Power Tools

Model và DbContext trong dự án hiện được viết theo cấu trúc SQL đã cung cấp; chưa được sinh bằng thao tác EF Core Power Tools trong Visual Studio.
Để thực hiện đúng bước scaffold của bài: sau khi chạy SQL, nhấp phải dự án → EF Core Power Tools → Reverse Engineer → chọn kết nối FitZone → chọn duy nhất dbo.HoiVien. Đặt context là FitZoneContext, namespace model là Cau2.Models và namespace context là Cau2.Data. Giữ cấu hình kết nối phù hợp server của bạn sau khi sinh lại.
Không dùng EnsureCreated hoặc migration để tạo database trong ứng dụng.

## Sử dụng

Chọn dòng để đưa dữ liệu lên form; Thêm tạo hội viên mới, Sửa cập nhật dòng đã chọn, Xóa yêu cầu xác nhận Yes/No. Làm mới xóa nội dung nhập và bộ lọc, tải lại toàn bộ danh sách. Tìm kiếm kết hợp họ tên chứa từ khóa và hạng thành viên; Tất cả bỏ điều kiện hạng.
Lỗi nhập liệu được hiển thị riêng theo trường bằng ErrorProvider và thông báo chi tiết. Không cho đăng ký dưới 15 tuổi.
