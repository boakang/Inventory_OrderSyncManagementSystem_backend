using Inventory_OrderSyncManagementSystem.Data;
using Inventory_OrderSyncManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _context;

        public SupplierService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<SupplierDto> GetAllSuppliers()
        {
            return _context.Suppliers.Select(s => new SupplierDto
            {
                SupplierID = s.SupplierID,
                Name = s.Name,
                ContactInfo = s.ContactInfo
            }).ToList();
        }

        public SupplierDto GetSupplierById(int id)
        {
            var s = _context.Suppliers.Find(id);
            if (s == null) return null!;

            return new SupplierDto
            {
                SupplierID = s.SupplierID,
                Name = s.Name,
                ContactInfo = s.ContactInfo
            };
        }

        public SupplierDto AddSupplier(SupplierDto supplierDto)
        {
            var supplier = new Supplier
            {
                Name = supplierDto.Name,
                ContactInfo = supplierDto.ContactInfo,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                LastModified = DateTime.Now
            };

            _context.Suppliers.Add(supplier);
            _context.SaveChanges();

            supplierDto.SupplierID = supplier.SupplierID;
            return supplierDto;
        }

        public SupplierDto UpdateSupplier(int id, SupplierDto supplierDto)
        {
            var existingSupplier = _context.Suppliers.Find(id);
            if (existingSupplier == null)
            {
                throw new KeyNotFoundException("Supplier not found.");
            }

            existingSupplier.Name = supplierDto.Name;
            existingSupplier.ContactInfo = supplierDto.ContactInfo;
            existingSupplier.ModifiedDate = DateTime.Now;
            existingSupplier.LastModified = DateTime.Now;

            _context.SaveChanges();

            return supplierDto;
        }

        public bool DeleteSupplier(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return false;
            }

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
            return true;
        }
    }
}
