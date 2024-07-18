using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeiConText.Interfaces
{
    public interface ICommand
    {
        Task ExecuteAsync(string[] args);
    }
}
