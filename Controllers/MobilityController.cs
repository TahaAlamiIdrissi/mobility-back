using AuthService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.Controllers
{
    [Route("/api/[controller]/")]
    public class MobilityController : ControllerBase
    {
        private readonly IMobility _mobilityService;

        public MobilityController(IMobility mobilityService)
        {
            this._mobilityService = mobilityService;
        }


        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Index()
        {
            return Ok("working");
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Create([FromBody] Mobility mobility)
        {
            var mob = this._mobilityService.AddMobility(mobility);
            if (mob == null)
            {
                return BadRequest("Error Happened ! please retry");
            }
            return CreatedAtAction(nameof(Create), new { mob });
        }

        [Route("[action]")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Edit([FromBody] Mobility mobility)
        {
            var mob = this._mobilityService.UpdateMobility(mobility);
            if (mob == null)
            {
                return NotFound("Mobility Not Found !");
            }
            return Ok("Mobility modified successfully !");
        }

        [Route("all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [Authorize(Roles = Role.Student)]
        public ActionResult Mobilities()
        {
            var mobilities = this._mobilityService.GetAllMobilities();
            return Ok(mobilities);
        }

        [Route("[action]/{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [Authorize(Roles = Role.Student)]
        public ActionResult GetMobility(int id)
        {
            var mobility = this._mobilityService.GetMobilityById(id);
            return Ok(mobility);
        }


        [HttpDelete]
        [Route("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Remove(int id)
        {
            var mobility = this._mobilityService.DeleteMobility(id);
            if (mobility == null)
            {
                return NotFound("Mobility not found");
            }
            return Ok(mobility);
        }


        // this is working but should not be like that ! 

        [Route("{country?}/{promotion?}/{studentName?}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Mobilties(string country, string promotion, string studentName)
        {
            var mobilities = this._mobilityService.GetMobilitiesMulticriteria(promotion, country, studentName);
            return Ok(mobilities);
        }

        [Route("[action]/{id}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Validate(int id)
        {
            var mobility = this._mobilityService.ValidateMobility(id);
            return Ok(mobility);
        }

    }
}