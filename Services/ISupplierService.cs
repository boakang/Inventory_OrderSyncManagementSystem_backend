using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public interface ISupplierService
    {
        IEnumerable<SupplierDto> GetAllSuppliers();
        SupplierDto GetSupplierById(int id);
        SupplierDto AddSupplier(SupplierDto supplierDto);
        SupplierDto UpdateSupplier(int id, SupplierDto supplierDto);
        bool DeleteSupplier(int id);
    }
}
