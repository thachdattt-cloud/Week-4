using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using tuan3.DTO;
using tuan3.models;
namespace tuan3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {


        private static List<Student> _students = new List<Student>
        {

            new Student {Id=1,Name="quang",Age=20},
            new Student {Id=2,Name="hung",Age=30},
            new Student {Id=3,Name="nguyen quang",Age=30},
            new Student {Id=4,Name="quang hung",Age=40}
        };

        private StudentResponseDto MapToDto(Student student)
        {
            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                Age= student.Age,
            };
        }

        [HttpGet("GetAllDto")]
        public ActionResult<List<StudentResponseDto>> GetAllDto()
        {
            var result = _students.Select(s => MapToDto(s)).ToList();
            return Ok(result);
        }




        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_students);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                return Ok(student);
            }
            return NotFound("ko thay id cua sinh vien");

        }

        [HttpPost]
        public IActionResult Create([FromBody] Student newStudent)
        {
            bool isExit = _students.Any(s => s.Id == newStudent.Id);

            if (isExit)
            {
                return BadRequest("id bi trung vui long nhap lai");
            }
            _students.Add(newStudent);

            return Ok(newStudent);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete( [FromRoute] int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound("ko tim thay ma ca xoa");
            }
             _students.Remove(student);
            return Ok(); 
        }
        [HttpPut]
        public  IActionResult Update(Student UpdateStudent)
        {
            var student = _students.FirstOrDefault(s => s.Id == UpdateStudent.Id);
            if(student == null)
            {
                return NotFound("ko tim thay id can sua");
            }

            student.Age = UpdateStudent.Age;
            student.Name = UpdateStudent.Name;


            return Ok(student);
        }

        [HttpGet("search")] 
        public IActionResult SearchStudent([FromQuery] string? name) {

            if (String.IsNullOrWhiteSpace(name))
            {
                return Ok(_students);
            }

            var student=_students.FirstOrDefault(s=>s.Name.Equals(name,StringComparison.OrdinalIgnoreCase));
            if( student == null)
            {
                return NotFound("ten ban nhap ko co trong danh sach");
            }
            return Ok($"ban muon tim student co name : {student.Name}");
        }

        [HttpGet("Search/name")]

        public IActionResult Search([FromQuery] string? name)
        {

            if (String.IsNullOrWhiteSpace(name))
            {
                return BadRequest("vui long nhap day du ten");
            }

            var student=_students.Where(s=>s.Name.Contains(name,StringComparison.OrdinalIgnoreCase)).ToList();


            if (student.Any()==false) return NotFound("ko tim thay sinh vien");

            string allName = string.Join(",", student.Select(s => s.Name));
            return Ok($"sinh vien ban muon tim co the la {allName}");
        }

        // dto

        [HttpPost("Create")]
        public ActionResult<StudentResponseDto> CreateDto([FromBody] CreateStudentDto dto)
        {
            int newId;
            if (_students.Count() ==0)
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

            var response = new StudentResponseDto
            {
                Id = newId,
                Name = newStudent.Name,
                Age = newStudent.Age
            };
            return Ok(response);

        }
    }
}