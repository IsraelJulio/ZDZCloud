using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categorias = await _context.Categorias.ToListAsync();
        return Ok(categorias);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Categoria categoria)
    {
        if (categoria.Nome.Length < 5)
            return BadRequest(new { errors = new[] { "Nome deve ter pelo menos 5 caracteres." } });

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Categoria categoria)
    {
        if (categoria.Nome.Length < 5)
            return BadRequest(new { errors = new[] { "Nome deve ter pelo menos 5 caracteres." } });

        var existing = await _context.Categorias.FindAsync(id);
        if (existing == null)
            return NotFound();

        existing.Nome = categoria.Nome;
        existing.Descricao = categoria.Descricao;
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
            return NotFound();

        var temProdutos = await _context.Produtos.AnyAsync(p => p.CategoriaId == id);
        if (temProdutos)
            return Conflict("Não é possível excluir uma categoria que possua produtos vinculados.");

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
