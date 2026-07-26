using Microsoft.AspNetCore.Mvc;
using tuan3.ApiResponse;
using tuan3.DTO;
using tuan3.models;

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
            new Student { Id = 4, Name = "quang hung", Age = 40 }
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
        public ActionResult<ApiResponse<List<StudentResponseDto>>> GetAll([FromQuery] string? keyword)
        {
          

         
            if (!string.IsNullOrWhiteSpace(keyword))
            {
               var  student = _students.Where(s => s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            var result =_students.Select(s => MapToDto(s)).ToList();
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
    }
}