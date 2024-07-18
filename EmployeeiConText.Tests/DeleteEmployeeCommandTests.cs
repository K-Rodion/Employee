using EmployeeiConText.Commands;
using EmployeeiConText.Interfaces;
using Moq;

namespace EmployeeiConText.Tests
{
    [TestFixture]
    public class DeleteEmployeeCommandTests
    {
        private Mock<IEmployeeRepository> mockRepository;
        private DeleteEmployeeCommand deleteEmployeeCommand;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new Mock<IEmployeeRepository>();
            deleteEmployeeCommand = new DeleteEmployeeCommand(mockRepository.Object);
        }

        [Test]
        public async Task ExecuteAsync_InvalidCommandFormat_PrintsErrorMessage()
        {
            using (var consoleOutput = new ConsoleOutput())
            {
                await deleteEmployeeCommand.ExecuteAsync(new string[] { "invalidCommand" });

                string output = consoleOutput.GetOutput();
                Assert.That(output, Is.EqualTo("Invalid delete command. Usage: -delete id:<id>\r\n"));
            }
        }

        [Test]
        public async Task ExecuteAsync_InvalidIdFormat_PrintsErrorMessage()
        {
            using (var consoleOutput = new ConsoleOutput())
            {
                await deleteEmployeeCommand.ExecuteAsync(new string[] { "id:invalidId" });

                string output = consoleOutput.GetOutput();
                Assert.That(output, Is.EqualTo("Invalid id format: invalidId. Please provide a valid integer id.\r\n"));
            }
        }

        [Test]
        public async Task ExecuteAsync_EmployeeNotFound_PrintsNotFoundMessage()
        {
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>())).Throws<KeyNotFoundException>();

            using (var consoleOutput = new ConsoleOutput())
            {
                await deleteEmployeeCommand.ExecuteAsync(new string[] { "id:1" });

                string output = consoleOutput.GetOutput();
                Assert.That(output, Is.EqualTo("Employee with Id 1 not found.\r\n"));
            }
        }

        [Test]
        public async Task ExecuteAsync_ValidCommand_DeletesEmployeeAndPrintsSuccessMessage()
        {
            var employee = new Employee { Id = 1, FirstName = "Test Employee" };
            mockRepository.Setup(repo => repo.GetAsync(1)).ReturnsAsync(employee);
            mockRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            using (var consoleOutput = new ConsoleOutput())
            {
                await deleteEmployeeCommand.ExecuteAsync(new string[] { "id:1" });

                string output = consoleOutput.GetOutput();
                Assert.That(output, Is.EqualTo("Employee with Id 1 has been successfully deleted.\r\n"));
            }

            mockRepository.Verify(repo => repo.GetAsync(1), Times.Once);
            mockRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_RepositoryThrowsException_PrintsErrorMessage()
        {
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Repository error"));

            using (var consoleOutput = new ConsoleOutput())
            {
                await deleteEmployeeCommand.ExecuteAsync(new string[] { "id:1" });

                string output = consoleOutput.GetOutput();
                Assert.That(output, Is.EqualTo("Error deleting employee: Repository error\r\n"));
            }
        }
    }

    public class ConsoleOutput : IDisposable
    {
        private StringWriter stringWriter;
        private TextWriter originalOutput;

        public ConsoleOutput()
        {
            stringWriter = new StringWriter();
            originalOutput = Console.Out;
            Console.SetOut(stringWriter);
        }

        public string GetOutput()
        {
            return stringWriter.ToString();
        }

        public void Dispose()
        {
            Console.SetOut(originalOutput);
            stringWriter.Dispose();
        }
    }
}