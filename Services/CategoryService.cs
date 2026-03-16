using Inventory_OrderSyncManagementSystem.Data;
using Inventory_OrderSyncManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<CategoryDto> GetAllCategories()
        {
            return _context.Categories.Select(c => new CategoryDto
            {
                CategoryID = c.CategoryID,
                Name = c.Name ?? string.Empty,
                Description = c.Description ?? string.Empty
            }).ToList();
        }

        public CategoryDto? GetCategoryById(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return null;
            }

            return new CategoryDto
            {
                CategoryID = category.CategoryID,
                Name = category.Name ?? string.Empty,
                Description = category.Description ?? string.Empty
            };
        }

        public CategoryDto AddCategory(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name ?? string.Empty,
                Description = categoryDto.Description ?? string.Empty
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            categoryDto.CategoryID = category.CategoryID;
            return categoryDto;
        }

        public CategoryDto? UpdateCategory(int id, CategoryDto categoryDto)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return null;
            }

            category.Name = categoryDto.Name ?? string.Empty;
            category.Description = categoryDto.Description ?? string.Empty;

            _context.SaveChanges();
            return new CategoryDto
            {
                CategoryID = category.CategoryID,
                Name = category.Name,
                Description = category.Description
            };
        }

        public bool DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return true;
        }
    }
}