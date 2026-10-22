using HomeWork_66_Sydykbekov_Talgat.Controllers;
using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public class CategoryService : ICategoryService
{
    private ICategoryService _categoryServiceImplementation;
    private readonly MyUserContext _context;

    public CategoryService(MyUserContext context)
    {
        _context = context;
    }
    public async Task<List<Category>> GetAllCategoryAsync()
    {
         return await _context.Categories.ToListAsync();
        
    }

    public async Task CreateAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }
}