using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public interface ICategoryService
    {
        IEnumerable<CategoryDto> GetAllCategories();
        CategoryDto? GetCategoryById(int id);
        CategoryDto AddCategory(CategoryDto categoryDto);
        CategoryDto? UpdateCategory(int id, CategoryDto categoryDto);
        bool DeleteCategory(int id);
    }
}