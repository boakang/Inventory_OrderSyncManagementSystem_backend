using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly List<CustomerDto> _customers = new();

        public IEnumerable<CustomerDto> GetAllCustomers()
        {
            return _customers;
        }

        public CustomerDto? GetCustomerById(int id)
        {
            return _customers.FirstOrDefault(c => c.CustomerID == id);
        }

        public CustomerDto AddCustomer(CustomerDto customerDto)
        {
            customerDto.CustomerID = _customers.Count + 1;
            _customers.Add(customerDto);
            return customerDto;
        }

        public CustomerDto? UpdateCustomer(int id, CustomerDto customerDto)
        {
            var existingCustomer = GetCustomerById(id);
            if (existingCustomer == null)
            {
                return null;
            }

            existingCustomer.FirstName = customerDto.FirstName;
            existingCustomer.LastName = customerDto.LastName;
            existingCustomer.Email = customerDto.Email;
            existingCustomer.Phone = customerDto.Phone;
            existingCustomer.Address = customerDto.Address;

            return existingCustomer;
        }

        public bool DeleteCustomer(int id)
        {
            var customer = GetCustomerById(id);
            if (customer == null)
            {
                return false;
            }

            _customers.Remove(customer);
            return true;
        }
    }
}