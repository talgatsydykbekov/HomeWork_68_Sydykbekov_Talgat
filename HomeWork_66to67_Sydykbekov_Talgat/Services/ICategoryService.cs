using HomeWork_66_Sydykbekov_Talgat.Models;

namespace HomeWork_66_Sydykbekov_Talgat.Services;

public interface ICategoryService
{
    public Task<List<Category>> GetAllCategoryAsync();
    public Task CreateAsync(Category category);
}