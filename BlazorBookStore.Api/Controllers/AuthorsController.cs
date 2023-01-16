﻿using AutoMapper;
using BlazorBookStore.Api.Data;
using BlazorBookStore.Api.Models.Author;
using BlazorBookStore.Api.Static;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorBookStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper mapper;
        private readonly ILogger<AuthorsController> logger;

        public AuthorsController(BookStoreDbContext context, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorReadOnlyDto>>> GetAuthors()
        {
            try
            {
                var authors = await _context.Authors.ToListAsync();
                var authorDto = mapper.Map<IEnumerable<AuthorReadOnlyDto>>(authors);
                return Ok(authorDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing Operation in {nameof(GetAuthors)}!");
                return StatusCode(500, Messages.Error500Message);
            }

        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorReadOnlyDto>> GetAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    logger.LogWarning($"Record not found {nameof(GetAuthor)} - ID: {id}!");
                    return NotFound();
                }

                var authorDto = mapper.Map<AuthorReadOnlyDto>(author);

                return Ok(authorDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing Operation in {nameof(GetAuthor)}!");
                return StatusCode(500, Messages.Error500Message); ;
            }

        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorUpdateDto authorDto)
        {
            if (id != authorDto.Id)
            {
                logger.LogWarning($"Update ID is Invalid in {nameof(PutAuthor)} - ID: {id}!");
                return BadRequest();
            }
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                logger.LogWarning($"{nameof(Author)} record not found in {nameof(PutAuthor)} - ID: {id}!");
                return NotFound();
            }

            mapper.Map(authorDto, author);
            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    logger.LogError(ex, $"Error Performing Operation in {nameof(PutAuthor)}!");
                    return StatusCode(500, Messages.Error500Message); ;
                }
            }
            return NoContent();
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AuthorCreateDto>> PostAuthor(AuthorCreateDto authorDto)
        {
            try
            {
                var author = mapper.Map<Author>(authorDto);
                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing Operation in {nameof(PostAuthor)}!", authorDto);
                return StatusCode(500, Messages.Error500Message); ;
            }

        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    logger.LogWarning($"{nameof(Author)} record not found in {nameof(DeleteAuthor)} - ID: {id}!");
                    return NotFound();
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing Operation in {nameof(DeleteAuthor)}!");
                return StatusCode(500, Messages.Error500Message); ;
            }
        }

        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(e => e.Id == id);
        }
    }
}
