using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EmployeeiConText.Interfaces;
using Newtonsoft.Json;

namespace EmployeeiConText
{
    public class JsonFileEmployeeRepository : IEmployeeRepository
    {
        private readonly string filePath;
        private List<Employee> employees = new List<Employee>();

        public JsonFileEmployeeRepository(string filePath, string[] args = null)
        {
            this.filePath = filePath;

            if (args != null)
            {
                GetEmployee(args);
            }
        }

        private void GetEmployee(string[] args)
        {
            foreach (var str in args)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Employee? employee = JsonConvert.DeserializeObject<Employee>(str);
                    if (employee != null)
                    {
                        employees.Add(employee);
                    }
                }
            }
        }

        public async Task LoadAsync()
        {
            if (File.Exists(filePath))
            {
                StreamReader? reader = null;

                try
                {
                    reader = new StreamReader(filePath);

                    string? str;

                    while ((str = await reader.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            Employee? employee = JsonConvert.DeserializeObject<Employee>(str);
                            if (employee != null)
                            {
                                employees.Add(employee);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    reader?.Dispose();
                }
            }
        }

        public async Task SaveAsync()
        {
            StreamWriter? writer = null;
            File.WriteAllText(filePath, string.Empty);

            try
            {
                writer = new StreamWriter(filePath, true);
                foreach (var employee in employees)
                {
                    string json = JsonConvert.SerializeObject(employee);
                    await writer.WriteLineAsync(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                writer?.Dispose();
            }
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            employee.Id = employees.Any() ? employees.Max(e => e.Id) + 1 : 1;
            employees.Add(employee);
            await SaveAsync();
            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            var existingEmployee = employees.FirstOrDefault(e => e.Id == employee.Id);
            if (existingEmployee == null)
            {
                throw new KeyNotFoundException($"Employee with Id {employee.Id} not found");
            }

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.SalaryPerHour = employee.SalaryPerHour;

            await SaveAsync();
            return existingEmployee;
        }

        public async Task<Employee> GetAsync(int id)
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with Id {id} not found");
            }
            return await Task.FromResult(employee);
        }

        public async Task DeleteAsync(int id)
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with Id {id} not found");
            }
            employees.Remove(employee);
            await SaveAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await Task.FromResult(employees);
        }
    }
}
