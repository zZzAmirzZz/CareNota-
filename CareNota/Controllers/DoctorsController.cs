//using AutoMapper;
//using CareNota.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace CareNota.Controllers
//{
    
//        [ApiController]
//        [Route("api/[controller]")]
//        public class DoctorController : ControllerBase
//        {
//            private readonly IDoctorService _service;
//            private readonly IMapper _mapper;

//            public DoctorController(IDoctorService service, IMapper mapper)
//            {
//                _service = service;
//                _mapper = mapper;
//            }

//            [HttpGet]
//            public IActionResult GetAll()
//            {
//                var result = _mapper.Map<List<DoctorDto>>(_service.GetAll());
//                return Ok(result);
//            }

//            [HttpGet("{id}")]
//            public IActionResult GetById(int id)
//            {
//                var doctor = _service.GetById(id);
//                if (doctor == null) return NotFound($"Doctor {id} not found.");
//                return Ok(_mapper.Map<DoctorDto>(doctor));
//            }

//            [HttpGet("specialty/{specialty}")]
//            public IActionResult GetBySpecialty(string specialty)
//            {
//                var result = _mapper.Map<List<DoctorDto>>(_service.GetDoctorsBySpecialty(specialty));
//                return Ok(result);
//            }

//            [HttpPost]
//            public IActionResult Add([FromBody] CreateDoctorDto dto)
//            {
//                if (!ModelState.IsValid) return BadRequest(ModelState);
//                _service.Add(_mapper.Map<Doctor>(dto));
//                return Ok("Doctor added successfully.");
//            }

//            [HttpPut("{id}")]
//            public IActionResult Update(int id, [FromBody] CreateDoctorDto dto)
//            {
//                var existing = _service.GetById(id);
//                if (existing == null) return NotFound($"Doctor {id} not found.");
//                _mapper.Map(dto, existing);
//                _service.Update(existing);
//                return NoContent();
//            }

//            [HttpDelete("{id}")]
//            public IActionResult Delete(int id)
//            {
//                var existing = _service.GetById(id);
//                if (existing == null) return NotFound($"Doctor {id} not found.");
//                _service.Delete(id);
//                return NoContent();
//            }
//        }
//    }

