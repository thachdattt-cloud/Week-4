using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using tuan3.ApiResponse;
using tuan3.DTO;
using tuan3.models;
using tuan3.Pagination;
namespace tuan3.Controllers
{
    [ApiController]
    [Route("api/students")] 
    public class StudentController : ControllerBase
    {
        private static List<Student> _students = new List<Student>
        {
            new Student { Id = 1, Name = "quang", Age = 20 },
            new Student { Id = 2, Name = "hung", Age = 30 },
            new Student { Id = 3, Name = "nguyen quang", Age = 30 },
            new Student { Id = 4, Name = "quang hung 1 ", Age = 40 },
            new Student { Id = 5, Name = "quang hung 2", Age = 40 },
            new Student { Id = 6, Name = "quang hung 3", Age = 40 },
            new Student { Id = 7, Name = "quang hung 4", Age = 40 },
            new Student { Id = 8, Name = "quang hung 5", Age = 40 },
            new Student { Id = 9, Name = "quang hung 6", Age = 40 },
            new Student { Id = 20, Name = "quang hung 7", Age = 40 },
            new Student { Id = 11, Name = "quang hung 8", Age = 40 },
            new Student { Id =12, Name = "quang hung 9", Age = 40 },
            new Student { Id = 13, Name = "quang hung 10", Age = 40 },
            new Student { Id = 14, Name = "quang hung 11", Age = 40 }
        };

        private StudentResponseDto MapToDto(Student student)
        {
            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                Age = student.Age
            };
        }

    
        [HttpGet]
        public async  Task <ActionResult<ApiResponse<List<StudentResponseDto>>>> GetAll([FromQuery] string? keyword)
        {
           await Task.Delay(2000);
            var student = _students;
         
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                student = student.Where(s => s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var result =student.Select(s => MapToDto(s)).ToList();
            return Ok(ApiResponse<List<StudentResponseDto>>.Ok(result, "Lay danh sach thanh cong"));
        }

    
        [HttpGet("{id}")]
        public ActionResult<ApiResponse<StudentResponseDto>> GetById([FromRoute] int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound(ApiResponse<StudentResponseDto>.Fail("Khong tim thay sinh vien"));
            }

            return Ok(ApiResponse<StudentResponseDto>.Ok(MapToDto(student), "Lay du lieu thanh cong"));
        }


        [HttpPost]
        public ActionResult<ApiResponse<StudentResponseDto>> Create([FromBody] CreateStudentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(ApiResponse<StudentResponseDto>.Fail("Ten khong duoc de trong"));
            }

            int newId;
            if (_students.Count() == 0)
            {
                newId = 1;
            }
            else
            {
                newId = _students.Max(s => s.Id) + 1;
            }

            var newStudent = new Student
            {
                Id = newId,
                Name = dto.Name,
                Age = dto.Age
            };

            _students.Add(newStudent);
            var response = MapToDto(newStudent);

            return CreatedAtAction(
                nameof(GetById),
                new { id = response.Id },
                ApiResponse<StudentResponseDto>.Ok(response, "Tao moi thanh cong"));
        }

    
        [HttpPut("{id}")]
        public ActionResult<ApiResponse<StudentResponseDto>> Update([FromRoute] int id, [FromBody] UpdateStudentDto dto)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound(ApiResponse<StudentResponseDto>.Fail("Khong tim thay sinh vien can sua"));
            }

            student.Name = dto.Name;
            student.Age = dto.Age;

            return Ok(ApiResponse<StudentResponseDto>.Ok(MapToDto(student), "Cap nhat thanh cong"));
        }

    
        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<string>> Delete([FromRoute] int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound(ApiResponse<string>.Fail("Khong tim thay sinh vien can xoa"));
            }

            _students.Remove(student);
            return Ok(ApiResponse<string>.Ok(null, "Xoa thanh cong"));
        }

        [HttpGet("Page")]
        public ActionResult<ApiResponse<PagedResult<StudentResponseDto>>>GetPage([FromQuery] PaginationQuery query){

            var queryStudent = _students.AsQueryable();
            if (!String.IsNullOrWhiteSpace(query.Keyword)){
            
                    queryStudent=queryStudent.Where(s=>s.Name.Contains(query.Keyword,StringComparison.OrdinalIgnoreCase));
                
            }

            var totalItems=queryStudent.Count();
            var skipCount = (query.PageNumber - 1) * query.PageSize;



            var items = queryStudent.Skip(skipCount)
                                  .Take(query.PageSize)
                                  .Select(s => MapToDto(s))
                                  .ToList();

            var pageResult = new PagedResult<StudentResponseDto>
            {

                Items=items,
                PageNumber=query.PageNumber,
                PageSize=query.PageSize,
                TotalItems=totalItems

            };



            return Ok(ApiResponse<PagedResult<StudentResponseDto>>.Ok(pageResult,"danh sach thong tin"));
        }
    }
}