using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extensions;
using api.Interfaces;
using api.MAppers;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace api.Controllers
{
    [ApiController]
    [Route("api/Comment")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentrepo;
        private readonly IStockRepository _stockrepo;
        private readonly UserManager<AppUser> _userManager;
        public CommentController(ICommentRepository commentrepo, IStockRepository stockrepo,UserManager<AppUser> userManager)
        {
            _commentrepo = commentrepo;
            _stockrepo = stockrepo;
            _userManager = userManager; 
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            var comments = await _commentrepo.GetAllAsync();
            
            var CommentDto = comments.Select(s => s.ToCommentDto());
            
            return Ok(CommentDto);
        }

        [HttpGet("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            var comment = await _commentrepo.GetByIdAsync(id);

            if(comment ==  null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto commentDto)
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            if(!await _stockrepo.StockExists(stockId))
            {
                return BadRequest("Stock does not exist");
            }

            var username = User.Getusername();
            var  AppUser = await _userManager.FindByNameAsync(username);

            var commentModel = commentDto.ToCommentFromCreate(stockId);
            commentModel.AppUserId = AppUser.Id;
           
            await _commentrepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new {id =commentModel.Id},commentModel.ToCommentDto());

        }

        [HttpPut("{id:int}")]
    
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        { 
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            var comment = await _commentrepo.UpdateAsync(id , updateDto.ToCommentFromUpdate());
            
            if(comment == null)
            {
                return NotFound("Comment not found");
            }
            return Ok(comment.ToCommentDto());
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                 return BadRequest(ModelState);

            var commentModel = await _commentrepo.DeleteAsync(id);

            if(commentModel == null)
            {
                return NotFound("Comment doesnot exist");
            }
            return Ok(commentModel);
        }
    }
}