using AutoMapper;
using CareNota.BLL.DTOs;
using CareNota.Models;
using Microsoft.AspNetCore.Mvc;

namespace CareNota.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;
        private readonly IMapper _mapper;

        public PatientController(IPatientService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _mapper.Map<List<PatientDto>>(_service.GetAll());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var patient = _service.GetById(id);
            if (patient == null) return NotFound($"Patient {id} not found.");
            return Ok(_mapper.Map<PatientDto>(patient));
        }

        [HttpPost]
        public IActionResult Add([FromBody] CreatePatientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _service.Add(_mapper.Map<Patient>(dto));
            return Ok("Patient added successfully.");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CreatePatientDto dto)
        {
            var existing = _service.GetById(id);
            if (existing == null) return NotFound($"Patient {id} not found.");
            _mapper.Map(dto, existing);
            _service.Update(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _service.GetById(id);
            if (existing == null) return NotFound($"Patient {id} not found.");
            _service.Delete(id);
            return NoContent();
        }
    }
}
