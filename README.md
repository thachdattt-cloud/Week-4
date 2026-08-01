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

### 4. Validate Dữ Liệu Đầu Vào (FluentValidation)
* **Mục đích:** Tách logic validate ra khỏi Controller, tự động chặn request không hợp lệ trước khi vào Action xử lý, tránh viết `if` kiểm tra thủ công rải rác.
* **Package sử dụng:** `FluentValidation.AspNetCore` (đăng ký validator + tự động validate).
* **Thực hiện (`Validators/`):**
  * `CreateStudentDtoValidator`: validate `Name` (không rỗng, tối đa 30 ký tự), `Age` (lớn hơn 19, nhỏ hơn 23).
  * `UpdateStudentDtoValidator`: rule tương tự, áp dụng cho `UpdateStudentDto`.
  * Message lỗi tùy chỉnh bằng tiếng Việt qua `.WithMessage(...)`.
* **Đồng bộ response lỗi validate với `ApiResponse<T>`:**
  * Cấu hình `ApiBehaviorOptions.InvalidModelStateResponseFactory` trong `Program.cs` — khi FluentValidation phát hiện dữ liệu không hợp lệ, nó tự động ghi lỗi vào `ModelState`; hàm này gom toàn bộ lỗi lại và trả về đúng cấu trúc `ApiResponse<object>.Fail(...)`, đồng nhất với các lỗi khác trong hệ thống (thay vì để mặc định trả cấu trúc `{ "errors": {...} }`).

---

## Cách Kiểm Tra Kết Quả — cập nhật thêm

| Endpoint | Mục đích test |
|---|---|
| `POST /api/students` với `Name` rỗng | FluentValidation tự chặn, response `ApiResponse` với message "Ten khong duoc de trong" |
| `POST /api/students` với `Age = -5` | Response message "Tuoi phai tu 19 den 23" |
| `PUT /api/students/{id}` với dữ liệu invalid | Tương tự Create, bị chặn trước khi vào Action |
| `POST`/`PUT` với dữ liệu hợp lệ | Vào Controller xử lý bình thường, không bị ảnh hưởng |

---

## Cấu Trúc File Cập Nhật (Ngày 3)

```text
tuan3/
├── Validators/
│   ├── CreateStudentDtoValidator.cs   # Rule validate cho CreateStudentDto
│   └── UpdateStudentDtoValidator.cs   # Rule validate cho UpdateStudentDto
└── Program.cs                         # Dang ky FluentValidation + ApiBehaviorOptions de dong bo response loi
```


