using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.MAppers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController :ControllerBase
    {
        private readonly IStockRepository _repo;
        public StockController(IStockRepository repo)
        {
            _repo = repo;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            var stock= await _repo.GetAllAsync(query);

            var stockDto = stock.Select(s => s.ToStockDto());

            return Ok(stock);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            var stock = await _repo.GetByIdAsync(id);

            if(stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            var stockModel = stockDto.ToStockFromCreateDto();

            await _repo.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new {id = stockModel.Id}, stockModel.ToStockDto());

        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update ([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            var stockModel = await _repo.UpdateAsync(id, updateDto);

            if(stockModel == null)
            {
                return NotFound();
            }

            return Ok(stockModel.ToStockDto());

        }

       
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            var stockModel = await _repo.DeleteSync(id);

            if(stockModel == null)
             {
                return NotFound();
             }

            return NoContent();
        }
    }
}