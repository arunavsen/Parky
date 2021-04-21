using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Model;
using ParkyAPI.Model.DTOs;
using ParkyAPI.Repository.IRepository;
using static System.Net.HttpStatusCode;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParkController : ControllerBase
    {
        public INationalParkRepository _npRepo;
        private IMapper _mapper;

        public NationalParkController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks();

            var objDto = new List<NationalParkDto>();

            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }

            return Ok(objDto);
        }

        [HttpGet("{nationalParkId:int}", Name= "GetNationalPark")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepo.GetNationalPark(nationalParkId);

            if (obj == null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<NationalParkDto>(obj);

            return Ok(objDto);
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_npRepo.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("","National Park Exist");
                return StatusCode(404, ModelState);
            }


            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("",$"Something went record when saving the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            //return CreatedAtRoute("GetNationalPark", new { nationalParkId = nationalParkObj.Id}, nationalParkObj);
            return CreatedAtRoute("GetNationalPark", new { nationalParkId = nationalParkObj.Id }, nationalParkObj);

        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto.Id != nationalParkId || nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_npRepo.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went record when updating the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            //return CreatedAtRoute("GetNationalPark", new { nationalParkId = nationalParkObj.Id}, nationalParkObj);
            return NoContent();

        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_npRepo.NationalParkExists(nationalParkId))
            {
                return NotFound();
            }

            var nationalParkObj = _npRepo.GetNationalPark(nationalParkId);
            if (!_npRepo.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went record when deleting the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}