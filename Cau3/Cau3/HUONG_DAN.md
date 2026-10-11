Chạy Database.sql trong SQL Server Management Studio để tạo SunriseHomestay và hai bảng trước khi mở ứng dụng.

Kết nối mặc định: Server=localhost;Database=SunriseHomestay;Trusted_Connection=True;TrustServerCertificate=True.
Có thể đặt biến môi trường SUNRISE_CONNECTION_STRING nếu dùng máy chủ khác.

Mở Cau3.slnx bằng Visual Studio và chạy project Cau3.
Ảnh được sao chép vào Images cạnh file thực thi; database chỉ lưu tên file.

Model và DbContext hiện được viết theo schema SQL, không được sinh bằng EF Core Power Tools.
Nếu bài yêu cầu thao tác scaffold bằng Power Tools: sau khi chạy SQL, dùng Reverse Engineer,
chọn database SunriseHomestay và hai bảng LoaiPhong, Phong; đặt namespace Cau3.Models,
context HomestayContext, tắt pluralize tên entity để giữ LoaiPhong và Phong.
Giữ tên DbSet LoaiPhongs, Phongs và navigation MaLoaiNavigation tương thích code form;
đặt lại OnConfiguring với chuỗi kết nối bên trên nếu tool sinh khác.