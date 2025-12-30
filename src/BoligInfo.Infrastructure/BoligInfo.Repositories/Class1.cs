using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoligInfo.Database;
using CSharpCornerApi.Models;

namespace BoligInfo.Repositories;

[Route("api/[controller]")] // Lowercase 'api' is convention
[ApiController]
public class CSharpCornerArticlesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<CSharpCornerArticlesController> _logger;

    public CSharpCornerArticlesController(
        AppDbContext context, 
        ILogger<CSharpCornerArticlesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CSharpCornerArticle>>> GetArticles()
    {
        return await _context.Articles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CSharpCornerArticle>> GetArticle(int id)
    {
        var article = await _context.Articles.FindAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        return article;
    }

    [HttpPost]
    public async Task<ActionResult<CSharpCornerArticle>> PostArticle(CSharpCornerArticle article)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        article.CreatedAt = DateTime.UtcNow; // Use UTC for consistency
        _context.Articles.Add(article);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating article");
            return StatusCode(500, "An error occurred while creating the article");
        }

        return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutArticle(int id, CSharpCornerArticle article)
    {
        if (id != article.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Find existing article to preserve certain fields
        var existingArticle = await _context.Articles.FindAsync(id);
        if (existingArticle == null)
        {
            return NotFound();
        }

        // Update only the fields that should be changed
        existingArticle.Title = article.Title;
        existingArticle.Content = article.Content;
        existingArticle.Author = article.Author;
        // Don't update CreatedAt

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ArticleExists(id))
            {
                return NotFound();
            }
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating article {Id}", id);
            return StatusCode(500, "An error occurred while updating the article");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var article = await _context.Articles.FindAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        _context.Articles.Remove(article);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting article {Id}", id);
            return StatusCode(500, "An error occurred while deleting the article");
        }

        return NoContent();
    }

    private bool ArticleExists(int id)
    {
        return _context.Articles.Any(e => e.Id == id);
    }
}