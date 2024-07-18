using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeiConText.Interfaces;

namespace EmployeeiConText.Commands
{
    public class AddEmployeeCommand : ICommand
    {
        private readonly IEmployeeRepository repository;

        public AddEmployeeCommand(IEmployeeRepository repository)
        {
            this.repository = repository;
        }

        public async Task ExecuteAsync(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Invalid add command. Usage: -add firstname:<name> lastname:<name> salary:<salary>");
                return;
            }

            var employee = new Employee();
            bool hasFirstName = false, hasLastName = false, hasSalary = false;

            foreach (var arg in args)
            {
                var kvp = arg.Split(':');
                if (kvp.Length != 2)
                {
                    Console.WriteLine($"Invalid argument format: {arg}. Expected format: key:value");
                    return;
                }

                string key = kvp[0].ToLower();
                string value = kvp[1];

                switch (key)
                {
                    case "firstname":
                        employee.FirstName = value;
                        hasFirstName = true;
                        break;
                    case "lastname":
                        employee.LastName = value;
                        hasLastName = true;
                        break;
                    case "salary":
                        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal salary))
                        {
                            employee.SalaryPerHour = salary;
                            hasSalary = true;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid salary format: {value}. Expected a decimal number.");
                            return;
                        }
                        break;
                    default:
                        Console.WriteLine($"Unknown parameter: {key}");
                        return;
                }
            }

            if (!hasFirstName || !hasLastName || !hasSalary)
            {
                Console.WriteLine("Missing required parameters. Please provide firstname, lastname, and salary.");
                return;
            }

            try
            {
                var addedEmployee = await repository.AddAsync(employee);
                Console.WriteLine($"Employee added successfully. Id: {addedEmployee.Id}, Name: {addedEmployee.FirstName} {addedEmployee.LastName}, Salary: {addedEmployee.SalaryPerHour}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding employee: {ex.Message}");
            }
        }
    }
}
