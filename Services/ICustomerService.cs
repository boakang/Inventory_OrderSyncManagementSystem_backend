using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public interface ICustomerService
    {
        IEnumerable<CustomerDto> GetAllCustomers();
        CustomerDto? GetCustomerById(int id);
        CustomerDto AddCustomer(CustomerDto customerDto);
        CustomerDto? UpdateCustomer(int id, CustomerDto customerDto);
        bool DeleteCustomer(int id);
    }
}