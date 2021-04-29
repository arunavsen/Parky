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
    [Route("api/v{version:apiVersion}/nationalpark")]
    [ApiVersion("2.0")]
    //[Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParkV2Controller : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParkV2Controller(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of NationalParks
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200,Type = typeof(List<NationalParkDto>))]
        public IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks().FirstOrDefault();

            return Ok(_mapper.Map<NationalParkDto>(objList));
        }
    }
}