using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalks.API.Controllers
{

    // https://localhost:44372/api/students
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // https://localhost:44372/api/students
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            string[] studentsNames = new string[] { "John Doe", "Jane Smith", "Alice Johnson" };

            return Ok(studentsNames);
        }
    }
}