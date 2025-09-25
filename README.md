# Website Cửa Hàng Linh Kiện Điện Tử - ASP.NET Core MVC

## Mô tả dự án
Đây là website bán linh kiện điện tử ASP.NET Core MVC

## Công nghệ sử dụng
- **Framework**: ASP.NET Core 9.0 MVC
- **Database**: SQLite (Entity Framework Core)
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Font Awesome, jQuery
- **ORM**: Entity Framework Core 9.0

## Chức năng chính

### 🏠 Trang chủ
- Hero section với thông tin giới thiệu
- Hiển thị danh mục sản phẩm
- Sản phẩm nổi bật
- Các tính năng ưu việt (giao hàng nhanh, bảo hành, hỗ trợ 24/7)
- Đánh giá khách hàng

### 📦 Quản lý sản phẩm
- Danh sách sản phẩm với phân trang
- Bộ lọc theo danh mục, giá, tìm kiếm
- Sắp xếp theo tên, giá, ngày tạo
- Chi tiết sản phẩm với thông tin đầy đủ
- Thêm vào giỏ hàng

### 🛒 Giỏ hàng
- Thêm sản phẩm vào giỏ hàng
- Cập nhật số lượng sản phẩm
- Xóa sản phẩm khỏi giỏ hàng
- Tính tổng tiền tự động
- Hiển thị số lượng sản phẩm trong header

### 👤 Quản lý người dùng
- Đăng ký tài khoản mới
- Đăng nhập/Đăng xuất
- Quản lý thông tin cá nhân

## Cấu trúc dự án

```
ElectronicsStore/
├── Controllers/
│   ├── HomeController.cs          # Trang chủ
│   ├── ProductsController.cs      # Quản lý sản phẩm
│   └── CartController.cs          # Giỏ hàng
├── Models/
│   ├── ApplicationUser.cs         # Model người dùng
│   ├── Category.cs               # Model danh mục
│   ├── Product.cs                # Model sản phẩm
│   ├── Cart.cs                   # Model giỏ hàng
│   └── Order.cs                  # Model đơn hàng
├── Views/
│   ├── Home/
│   │   └── Index.cshtml          # Trang chủ
│   ├── Products/
│   │   ├── Index.cshtml          # Danh sách sản phẩm
│   │   └── Details.cshtml        # Chi tiết sản phẩm
│   ├── Cart/
│   │   └── Index.cshtml          # Giỏ hàng
│   └── Shared/
│       └── _Layout.cshtml        # Layout chung
├── Areas/Identity/               # Trang đăng nhập/đăng ký
├── Data/
│   └── ApplicationDbContext.cs   # Database context
└── wwwroot/                      # Static files
```

## Hướng dẫn cài đặt và chạy

### 1. Yêu cầu hệ thống
- .NET 9.0 SDK
- Visual Studio 2022 hoặc VS Code
- Git

### 2. Clone và cài đặt
```bash
# Clone repository (nếu có)
git clone [repository-url]
cd ElectronicsStore

# Restore packages
dotnet restore

# Tạo database
dotnet ef database update

# Chạy ứng dụng
dotnet run
```

### 3. Truy cập ứng dụng
- Mở trình duyệt và truy cập: `https://localhost:5001` hoặc `http://localhost:5000`

## Dữ liệu mẫu
Hệ thống đã được cấu hình với dữ liệu mẫu bao gồm:

### Danh mục sản phẩm:
- Vi xử lý (CPU)
- Bo mạch chủ (Mainboard)
- RAM (Bộ nhớ trong)
- Ổ cứng (HDD/SSD)
- Card đồ họa (VGA)
- Nguồn máy tính (PSU)

### Sản phẩm mẫu:
- Intel Core i5-12400F - 4,500,000đ
- AMD Ryzen 5 5600X - 5,200,000đ
- ASUS ROG STRIX B550-F - 4,800,000đ
- Corsair Vengeance LPX 16GB - 1,800,000đ
- Samsung 980 PRO 1TB - 3,200,000đ
- NVIDIA RTX 4060 Ti - 12,500,000đ
- Corsair RM750x - 2,800,000đ
- WD Blue 2TB - 1,500,000đ

## Tính năng đã hoàn thành

### ✅ Frontend
- [x] Trang chủ responsive với Bootstrap 5
- [x] Danh sách sản phẩm với bộ lọc và tìm kiếm
- [x] Chi tiết sản phẩm với gallery hình ảnh
- [x] Giỏ hàng với AJAX cập nhật real-time
- [x] Trang đăng nhập/đăng ký
- [x] Header với search và cart counter
- [x] Footer với thông tin liên hệ

### ✅ Backend
- [x] ASP.NET Core Identity cho authentication
- [x] Entity Framework với SQLite database
- [x] Repository pattern cho data access
- [x] RESTful API endpoints cho cart operations
- [x] Seed data cho categories và products
- [x] Model validation và error handling

### ✅ Database
- [x] User management với ASP.NET Identity
- [x] Product catalog với categories
- [x] Shopping cart với cart items
- [x] Order management system
- [x] Foreign key relationships
- [x] Database migrations

## Hướng dẫn sử dụng

### 1. Duyệt sản phẩm
- Truy cập trang chủ để xem sản phẩm nổi bật
- Click "Sản phẩm" để xem toàn bộ danh sách
- Sử dụng bộ lọc theo danh mục và giá
- Tìm kiếm sản phẩm theo tên

### 2. Thêm vào giỏ hàng
- Click nút "Chi tiết" để xem thông tin sản phẩm
- Chọn số lượng và click "Thêm vào giỏ hàng"
- Hoặc click nút giỏ hàng trực tiếp từ danh sách sản phẩm

### 3. Quản lý giỏ hàng
- Click icon giỏ hàng ở header để xem giỏ hàng
- Cập nhật số lượng sản phẩm
- Xóa sản phẩm không cần thiết
- Xem tổng tiền và tiến hành thanh toán

### 4. Đăng ký/Đăng nhập
- Click "Đăng nhập" ở header
- Đăng ký tài khoản mới hoặc đăng nhập
- Cần đăng nhập để sử dụng chức năng giỏ hàng

## Tính năng sẽ phát triển

### 🔄 Đang phát triển
- [ ] Checkout và thanh toán online
- [ ] Quản lý đơn hàng
- [ ] Đánh giá và review sản phẩm
- [ ] Wishlist (danh sách yêu thích)
- [ ] Admin panel quản lý sản phẩm

### 🚀 Tương lai
- [ ] Tích hợp payment gateway (VNPay, MoMo)
- [ ] Email notifications
- [ ] SMS notifications
- [ ] Real-time chat support
- [ ] Mobile app với Xamarin/MAUI

## Liên hệ và hỗ trợ
- **Người phát triển**: Đoàn Phước Miền
- **Email**: [email@example.com]
- **Dự án**: Website Linh Kiện Điện Tử ASP.NET Core MVC

## License
Dự án này được phát triển cho mục đích học tập và thương mại.

---
*Cập nhật lần cuối: 23/09/2024*
