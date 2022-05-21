using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using RabbitMQ_API.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RabbitMQ_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private List<Employee> employees { get; set; }

        private readonly IPublisher _publisher;
        public EmployeeController(IPublisher publisher)
        {
            employees = new List<Employee> { new Employee { Id=1,Name="test",Surname="test",Age=1},
                                             new Employee {Id=2,Name="test2",Surname="test2",Age=2},
                                             new Employee {Id=3,Name="test3",Surname="test3",Age=3}};
            _publisher = publisher;
        }
        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<Employee>>> Get()
        {
            _publisher.Publish(JsonConvert.SerializeObject(employees),"report.employee",null);
            return employees;
        }

        [HttpGet("Get/{id}")]
        public async Task<ActionResult<Employee>> Get(int id)
        {
            var employee = employees.FirstOrDefault(x => x.Id == id);
            _publisher.Publish(JsonConvert.SerializeObject(employee), "report.employee", null);
            return Ok(employees.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost("Post")]
        public async Task<ActionResult<Employee>> Post([FromBody] Employee employee )
        {
            employees.Add(employee);
            return Ok(employee);
        }

        [HttpPut("Put")]
        public async Task<ActionResult<Employee>> Put([FromBody] Employee employee)
        {
            var employeeForUpdate = employees.FirstOrDefault(x => x.Id == employee.Id);
            int index = employees.IndexOf(employeeForUpdate);
            employees[index] = employee;
            return Ok(employeeForUpdate);
        }

        [HttpDelete("Delete/{id}")]
        public void Delete(int id)
        {
            var employeeForDelete = employees.FirstOrDefault(x => x.Id == id);
            employees.Remove(employeeForDelete);
        }
    }
}
