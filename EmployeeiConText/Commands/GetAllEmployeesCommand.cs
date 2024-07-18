using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeiConText.Interfaces;

namespace EmployeeiConText.Commands
{
    public class GetAllEmployeesCommand : ICommand
    {
        private readonly IEmployeeRepository repository;

        public GetAllEmployeesCommand(IEmployeeRepository repository)
        {
            this.repository = repository;
        }

        public async Task ExecuteAsync(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("The -getall command doesn't accept any arguments.");
                return;
            }

            try
            {
                var employees = await repository.GetAllAsync();

                if (!employees.Any())
                {
                    Console.WriteLine("No employees found.");
                    return;
                }

                DisplayEmployees(employees);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving employees: {ex.Message}");
            }
        }

        private void DisplayEmployees(IEnumerable<Employee> employees)
        {

            foreach (var employee in employees)
            {
                Console.WriteLine($"Id = {employee.Id}, FirstName = {employee.FirstName}, LastName = {employee.LastName}, SalaryPerHour = {employee.SalaryPerHour}");
            }
        }
    }
}
