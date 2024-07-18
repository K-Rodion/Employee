using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeiConText.Interfaces;

namespace EmployeeiConText.Commands
{
    public class GetEmployeeCommand : ICommand
    {
        private readonly IEmployeeRepository repository;

        public GetEmployeeCommand(IEmployeeRepository repository)
        {
            this.repository = repository;
        }

        public async Task ExecuteAsync(string[] args)
        {
            if (args.Length != 1 || !args[0].StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Invalid get command. Usage: -get id:<id>");
                return;
            }

            string idString = args[0].Substring(3);

            if (!int.TryParse(idString, out int id))
            {
                Console.WriteLine($"Invalid id format: {idString}. Please provide a valid integer id.");
                return;
            }

            try
            {
                var employee = await repository.GetAsync(id);
                DisplayEmployee(employee);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Employee with Id {id} not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving employee: {ex.Message}");
            }
        }

        private void DisplayEmployee(Employee employee)
        {
            Console.WriteLine($"Id = {employee.Id}, FirstName = {employee.FirstName}, LastName = {employee.LastName}, SalaryPerHour = {employee.SalaryPerHour}");
        }
    }
}
