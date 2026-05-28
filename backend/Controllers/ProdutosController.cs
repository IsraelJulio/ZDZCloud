using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .ToListAsync();
        return Ok(produtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Produto produto)
    {
        if (produto.Nome.Length < 5)
            return BadRequest(new { errors = new[] { "Nome deve ter pelo menos 5 caracteres." } });

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        await _context.Entry(produto).Reference(p => p.Categoria).LoadAsync();
        return CreatedAtAction(nameof(GetAll), new { id = produto.Id }, produto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Produto produto)
    {
        if (produto.Nome.Length < 5)
            return BadRequest(new { errors = new[] { "Nome deve ter pelo menos 5 caracteres." } });

        var existing = await _context.Produtos.FindAsync(id);
        if (existing == null)
            return NotFound();

        existing.Nome = produto.Nome;
        existing.Descricao = produto.Descricao;
        existing.Preco = produto.Preco;
        existing.CategoriaId = produto.CategoriaId;
        await _context.SaveChangesAsync();

        await _context.Entry(existing).Reference(p => p.Categoria).LoadAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
            return NotFound();

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
