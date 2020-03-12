using System;
using System.Threading.Tasks;
using AnbarUchotu.Models.Dtos;
using AnbarUchotu.Repos.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnbarUchotu.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepo _repo;

        public ProductsController(IProductsRepo repo)
        {
            this._repo = repo;
        }

        [HttpGet("{guid}")]
        public async Task<IActionResult> Product(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return BadRequest("Guid is not provided.");
            }

            var product = await _repo.Product(guid);

            if (product != null)
            {
                return Ok(product);
            }
            return NotFound();
        }

        [HttpGet("all")]
        public async Task<IActionResult> Products(int rn, int c)
        {
            if (rn * c <= 0)
            {
                return BadRequest();
            }

            var products = await _repo.Products(rn, c);

            if (products != null)
            {
                return Ok(products);
            }
            return NotFound();
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody]ProductRegisterDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _repo.Register(product);

            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(500, "Unable to add product. Try again later.");
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody]ProductUpdateDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _repo.Update(product);

            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(500, "Something went wrong. Try again later");
        }

        [HttpDelete("delete/{guid:guid}")]
        public async Task<IActionResult> Delete(string guid)
        {
            if (String.IsNullOrEmpty(guid))
            {
                return BadRequest();
            }

            var result = await _repo.Delete(guid);

            if (result)
            {
                return Ok("Record deleted succesfully.");
            }
            return StatusCode(500, "Something went wrong. Try again later");
        }
    }
}