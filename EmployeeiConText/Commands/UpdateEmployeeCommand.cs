using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeiConText.Interfaces;

namespace EmployeeiConText.Commands
{
    public class UpdateEmployeeCommand : ICommand
    {
        private readonly IEmployeeRepository repository;

        public UpdateEmployeeCommand(IEmployeeRepository repository)
        {
            this.repository = repository;
        }

        public async Task ExecuteAsync(string[] args)
        {
            if (args.Length < 2 || !args[0].StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Invalid update command. Usage: -update id:<id> [firstname:<name>] [lastname:<name>] [salary:<salary>]");
                return;
            }

            if (!int.TryParse(args[0].Split(':')[1], out int id))
            {
                Console.WriteLine("Invalid id format. Please provide a valid integer id.");
                return;
            }

            try
            {
                var employee = await repository.GetAsync(id);
                bool hasChanges = false;

                foreach (var arg in args.Skip(1))
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
                            if (employee.FirstName != value)
                            {
                                employee.FirstName = value;
                                hasChanges = true;
                            }
                            break;
                        case "lastname":
                            if (employee.LastName != value)
                            {
                                employee.LastName = value;
                                hasChanges = true;
                            }
                            break;
                        case "salary":
                            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal salary))
                            {
                                if (employee.SalaryPerHour != salary)
                                {
                                    employee.SalaryPerHour = salary;
                                    hasChanges = true;
                                }
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

                if (hasChanges)
                {
                    var updatedEmployee = await repository.UpdateAsync(employee);
                    Console.WriteLine($"Employee updated successfully. Id: {updatedEmployee.Id}, Name: {updatedEmployee.FirstName} {updatedEmployee.LastName}, Salary: {updatedEmployee.SalaryPerHour}");
                }
                else
                {
                    Console.WriteLine("No changes were made to the employee.");
                }
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Employee with Id {id} not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee: {ex.Message}");
            }
        }
    }
}
