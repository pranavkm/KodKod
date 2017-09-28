using Microsoft.AspNetCore.Mvc;

namespace KodKodSample
{
    [ApiExplorerSettings]
    [ApiController]
    [Route("pet")]
    public class PetController : Controller
    {
        [HttpPost]
        public ActionResult Post([FromBody] Pet pet)
        {
            return Ok();
        }

        [HttpPut]
        public ActionResult Put([FromBody] Pet pet)
        {
            if (pet.Id == 0)
            {
                return NotFound();
            }

            return Ok(new Pet());
        }

        [HttpGet("findByStatus")]
        public ActionResult<Pet> FindByStatus([FromQuery] Status status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("findByTags")]
        public ActionResult<Pet> FindByTags([FromQuery] string[] tags)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id, [FromHeader(Name = "api_key")] string apiKey)
        {
            if (id == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<Pet> Get([FromRoute] int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
