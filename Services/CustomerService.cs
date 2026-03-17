using Inventory_OrderSyncManagementSystem.Data;
using Inventory_OrderSyncManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CustomerDto> GetAllCustomers()
        {
            return _context.Customers.Select(c => new CustomerDto
            {
                CustomerID = c.CustomerID,
                FirstName = c.FirstName ?? string.Empty,
                LastName = c.LastName ?? string.Empty,
                Email = c.Email ?? string.Empty,
                Phone = c.Phone ?? string.Empty,
                Address = c.Address ?? string.Empty
            }).ToList();
        }

        public CustomerDto? GetCustomerById(int id)
        {
            var c = _context.Customers.Find(id);
            if (c == null) return null;

            return new CustomerDto
            {
                CustomerID = c.CustomerID,
                FirstName = c.FirstName ?? string.Empty,
                LastName = c.LastName ?? string.Empty,
                Email = c.Email ?? string.Empty,
                Phone = c.Phone ?? string.Empty,
                Address = c.Address ?? string.Empty
            };
        }

        public CustomerDto AddCustomer(CustomerDto customerDto)
        {
            var customer = new Customer
            {
                FirstName = customerDto.FirstName ?? string.Empty,
                LastName = customerDto.LastName ?? string.Empty,
                Email = customerDto.Email ?? string.Empty,
                Phone = customerDto.Phone ?? string.Empty,
                Address = customerDto.Address ?? string.Empty,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                LastModified = DateTime.Now
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            customerDto.CustomerID = customer.CustomerID;
            return customerDto;
        }

        public CustomerDto? UpdateCustomer(int id, CustomerDto customerDto)
        {
            var existingCustomer = _context.Customers.Find(id);
            if (existingCustomer == null) return null;

            existingCustomer.FirstName = customerDto.FirstName ?? existingCustomer.FirstName;
            existingCustomer.LastName = customerDto.LastName ?? existingCustomer.LastName;
            existingCustomer.Email = customerDto.Email ?? existingCustomer.Email;
            existingCustomer.Phone = customerDto.Phone ?? existingCustomer.Phone;
            existingCustomer.Address = customerDto.Address ?? existingCustomer.Address;
            existingCustomer.ModifiedDate = DateTime.Now;
            existingCustomer.LastModified = DateTime.Now;

            _context.SaveChanges();

            return customerDto;
        }

        public bool DeleteCustomer(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return true;
        }
    }
}