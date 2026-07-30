# Bài Tập Tuần 4 - Xử Lý Bất Đồng Bộ & Middleware Trong ASP.NET Core

Dự án tiếp tục phát triển từ tuần 3, tập trung vào việc tối ưu hóa hiệu năng ứng dụng với **Asynchronous Programming (Async/Await)** và xây dựng **Custom Middleware** để ghi log hệ thống.

---

## Kiến Thức Mới Trong Tuần 4

### 1. Xử Lý Bất Đồng Bộ (Async / Await)
* **Mục đích:** Giúp giải phóng thread xử lý của web server trong thời gian chờ tác vụ I/O (database, external API, delay...), cải thiện khả năng mở rộng (scalability) của ứng dụng.
* **Thực hiện (`StudentController.cs`):**
  * Chuyển đổi action `GetAll` sang kiểu trả về `async Task<ActionResult<...>>`.
  * Sử dụng `await Task.Delay(2000)` để giả lập tác vụ bất đồng bộ tốn thời gian mà không gây nghẽn (block) thread.

### 2. Custom Middleware (Request Logging)
* **Mục đích:** Can thiệp vào HTTP Request Pipeline để đo thời gian xử lý của mỗi request và ghi log thông tin cơ bản.
* **Thực hiện (`RequestLoggingMiddleware.cs`):**
  * Sử dụng `Stopwatch` để tính thời gian thực thi (duration).
  * Ghi nhận các thông tin: **HTTP Method**, **Request Path**, **Response Status Code**, và **Execution Time (ms)**.
  * Gọi `await _next(context)` để chuyển quyền xử lý tiếp theo cho Middleware kế tiếp trong pipeline.

---Cách Kiểm Tra Kết Quả (Testing)
Khởi chạy ứng dụng: dotnet run hoặc bấm Run trong Visual Studio.

Gọi API: Gửi request GET /api/student (qua Swagger, Postman hoặc file .http).

Quan sát Console Log:

Kiểm tra thông báo bắt đầu xử lý và kết quả log thời gian phản hồi (khoảng > 2000ms do delay).

Ví dụ output trên Console:

Plaintext
[log] bat dau xu ly :
[GET] /api/student => 200 (2015ms)

3. Xử Lý Ngoại Lệ Toàn Cục (Global Exception Handling)
Mục đích: Chuẩn hóa response lỗi cho toàn bộ API, tránh mỗi Action tự viết NotFound()/BadRequest() rải rác, đồng thời ghi log lại lỗi để debug sau này.
Custom Exception (Exceptions/):
NotFoundException: ném ra khi không tìm thấy dữ liệu (thay cho return NotFound(...)).
BadRequestException: ném ra khi dữ liệu đầu vào không hợp lệ (thay cho return BadRequest(...)).
Thực hiện (GlobalExceptionMiddleware.cs):
Bọc try/catch quanh toàn bộ pipeline (await _next(context)), bắt mọi exception xảy ra ở Controller hoặc middleware phía sau.
Map từng loại exception sang đúng status code: NotFoundException → 404, BadRequestException → 400, còn lại (lỗi không lường trước) → 500.
Với lỗi 500, ẩn message thật, chỉ trả thông báo chung chung cho client — tránh lộ thông tin hệ thống.
Dùng ILogger<GlobalExceptionMiddleware> (được ASP.NET Core tự động inject) để ghi log lỗi ra Console, kèm ex để có stack trace đầy đủ khi debug.
Response lỗi vẫn dùng chung ApiResponse<T>.Fail(...) như response thành công — đồng nhất cấu trúc { success, message, data } cho cả 2 trường hợp.
Đăng ký (Program.cs): GlobalExceptionMiddleware đặt đầu tiên trong pipeline, trước cả RequestLoggingMiddleware, để bắt được lỗi từ mọi nơi.

Cách Kiểm Tra Kết Quả (Testing) — cập nhật

Gọi các endpoint sau qua Swagger/Postman:

Endpoint	Mục đích test
GET /api/students/9999	Test lỗi 404 (id không tồn tại) → response { success: false, message: "khong tim thay sinh vien" }
POST /api/students với Name rỗng	Test lỗi 400 → response { success: false, message: "ten khong duoc de trong kk" }
GET /api/students/test-error-500	Test lỗi 500 → response ẩn message thật, chỉ hiện thông báo chung; đồng thời Console in log Error kèm stack trace


Ví dụ output Console khi gọi lỗi 500:

[error] : loi 500
[GET] /api/students/test-error-500 => 500 (3ms)

 Endpoint test-error-500 chỉ dùng để test, cần xóa trước khi nộp bài chính thức.

Cấu Trúc File Cập Nhật (Ngày 2)
text
tuan3/
├── Controllers/
│   └── StudentController.cs         # Cac Action nem Custom Exception thay vi return truc tiep; them GetPage()
├── Exceptions/
│   ├── NotFoundException.cs         # Exception rieng cho loi 404
│   └── BadRequestException.cs       # Exception rieng cho loi 400
├── Middlewares/
│   ├── RequestLoggingMiddleware.cs
│   └── GlobalExceptionMiddleware.cs # Bat toan bo exception, chuan hoa response loi, ghi log qua ILogger

└── Program.cs                       # Dang ky GlobalExceptionMiddleware DAU TI



