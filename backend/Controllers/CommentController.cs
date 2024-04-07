using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Comment;
using backend.Interfaces;
using backend.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var comments = await _commentRepo.GetAllAsync();
            var commentDto = comments.Select(s => s.ToCommentDto());
            return Ok(commentDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto comment) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (!await _stockRepo.StockExists(stockId)) return BadRequest("Stock does not exist");
            var commentModel = comment.ToCommentFromCreateDto(stockId);
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut("{stockId:int}")]
        public async Task<IActionResult> Update([FromRoute] int stockId, [FromBody] UpdateCommentDto comment) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            var commentModel = await _commentRepo.UpdateAsync(stockId, comment.ToCommentFromUpdate());
            if (commentModel == null) return NotFound("comment to update not found");
            return Ok(commentModel.ToCommentDto()); 
        }

        [HttpDelete("{stockId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int stockId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            var commentModel = await _commentRepo.DeleteAsync(stockId);
            
            if (commentModel == null) return NotFound("Comment does not exist");
            
            return Ok(commentModel.ToCommentDto());
        }
    }
}