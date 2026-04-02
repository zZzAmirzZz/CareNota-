using AutoMapper;
using CareNota.Models;
using Microsoft.AspNetCore.Mvc;

namespace CareNota.Controllers
{

        [ApiController]
        [Route("api/[controller]")]
        public class AppointmentController : ControllerBase
        {
            private readonly IAppointmentService _service;
            private readonly IMapper _mapper;

            public AppointmentController(IAppointmentService service, IMapper mapper)
            {
                _service = service;
                _mapper = mapper;
            }

            [HttpGet]
            public IActionResult GetAll()
            {
                var result = _mapper.Map<List<AppointmentDto>>(_service.GetAll());
                return Ok(result);
            }

            [HttpGet("{id}")]
            public IActionResult GetById(int id)
            {
                var appointment = _service.GetById(id);
                if (appointment == null) return NotFound($"Appointment {id} not found.");
                return Ok(_mapper.Map<AppointmentDto>(appointment));
            }

            [HttpGet("date/{date}")]
            public IActionResult GetByDate(DateTime date)
            {
                var result = _mapper.Map<List<AppointmentDto>>(_service.GetAppointmentsByDate(date));
                return Ok(result);
            }

            [HttpPost]
            public IActionResult Add([FromBody] CreateAppointmentDto dto)
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                _service.Add(_mapper.Map<Appointment>(dto));
                return Ok("Appointment added successfully.");
            }

            [HttpPut("{id}")]
            public IActionResult Update(int id, [FromBody] CreateAppointmentDto dto)
            {
                var existing = _service.GetById(id);
                if (existing == null) return NotFound($"Appointment {id} not found.");
                _mapper.Map(dto, existing);
                _service.Update(existing);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                var existing = _service.GetById(id);
                if (existing == null) return NotFound($"Appointment {id} not found.");
                _service.Delete(id);
                return NoContent();
            }
        }

    }

