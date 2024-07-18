
using EmployeeiConText.Commands;
using EmployeeiConText.Interfaces;

namespace EmployeeiConText
{
    public class Program
    {
        private static IEmployeeRepository employeeRepository;
        private static readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

        public static async Task Main(string[] args)
        {
            employeeRepository = new JsonFileEmployeeRepository("employees.json");
            await employeeRepository.LoadAsync();

            InitializeCommands();

            while (true)
            {
                Console.Write("Enter command: ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                string[] commandParts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string cmd = commandParts[0].ToLower();
                string[] commandArgs = commandParts.Skip(1).ToArray();

                await ProcessCommandAsync(cmd, commandArgs);
            }
        }

        private static void InitializeCommands()
        {
            commands[CommandConstants.Add] = new AddEmployeeCommand(employeeRepository);
            commands[CommandConstants.Update] = new UpdateEmployeeCommand(employeeRepository);
            commands[CommandConstants.Get] = new GetEmployeeCommand(employeeRepository);
            commands[CommandConstants.Delete] = new DeleteEmployeeCommand(employeeRepository);
            commands[CommandConstants.GetAll] = new GetAllEmployeesCommand(employeeRepository);
        }

        private static async Task ProcessCommandAsync(string command, string[] args)
        {
            if (commands.TryGetValue(command, out ICommand cmd))
            {
                await cmd.ExecuteAsync(args);
            }
            else
            {
                Console.WriteLine("Unknown command");
            }
        }
    }
}
