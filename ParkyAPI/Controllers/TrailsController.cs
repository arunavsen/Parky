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
    [Route("api/Trails")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of Trails
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200,Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var objList = _trailRepo.GetTrails();

            var objDto = new List<TrailDto>();

            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(objDto);
        }

        /// <summary>
        /// Get Individual Trail
        /// </summary>
        /// <param name="trailId">Id of the Trail</param>
        /// <returns></returns>
        [HttpGet("{trailId:int}", Name= "GetTrail")]
        [ProducesResponseType(200,Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int trailId)
        {
            var obj = _trailRepo.GetTrail(trailId);

            if (obj == null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<TrailDto>(obj);

            return Ok(objDto);
        }

        [HttpPost]
        [ProducesResponseType(201,Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepo.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("","National Park Exist");
                return StatusCode(404, ModelState);
            }


            var trailObj = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.CreateTrail(trailObj))
            {
                ModelState.AddModelError("",$"Something went record when saving the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            //return CreatedAtRoute("GetTrail", new { TrailId = TrailObj.Id}, TrailObj);
            return CreatedAtRoute("GetTrail", new { TrailId = trailObj.Id }, trailObj);

        }

        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto.Id != trailId || trailDto == null)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went record when updating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            //return CreatedAtRoute("GetTrail", new { TrailId = TrailObj.Id}, TrailObj);
            return NoContent();

        }

        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepo.TrailExists(trailId))
            {
                return NotFound();
            }

            var trailObj = _trailRepo.GetTrail(trailId);
            if (!_trailRepo.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went record when deleting the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}