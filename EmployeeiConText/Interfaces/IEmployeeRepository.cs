using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeiConText.Interfaces
{
    public interface IEmployeeRepository
    {
        Task LoadAsync();
        Task SaveAsync();
        Task<Employee> AddAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task<Employee> GetAsync(int id);
        Task DeleteAsync(int id);
        Task<IEnumerable<Employee>> GetAllAsync();
    }
}
