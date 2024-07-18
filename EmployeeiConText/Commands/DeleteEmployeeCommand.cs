using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeiConText.Interfaces;

namespace EmployeeiConText.Commands
{
    public class DeleteEmployeeCommand : ICommand
    {
        private readonly IEmployeeRepository repository;

        public DeleteEmployeeCommand(IEmployeeRepository repository)
        {
            this.repository = repository;
        }

        public async Task ExecuteAsync(string[] args)
        {
            if (args.Length != 1 || !args[0].StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Invalid delete command. Usage: -delete id:<id>");
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

                await repository.DeleteAsync(id);
                Console.WriteLine($"Employee with Id {id} has been successfully deleted.");
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Employee with Id {id} not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting employee: {ex.Message}");
            }
        }
    }
}
