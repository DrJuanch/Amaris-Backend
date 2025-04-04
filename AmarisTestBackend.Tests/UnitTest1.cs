using System.Net;
using Moq;
using Moq.Protected;
using System.Text.Json;
using Xunit;

public class EmployeeServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://dummy.restapiexample.com/")
        };

        _employeeService = new EmployeeService(_httpClient);
    }

    [Fact]
    public async Task GetEmployeesAsync_ShouldReturnEmployees_WhenApiResponseIsValid()
    {
        // Arrange
        var mockApiResponse = new
        {
            Data = new[]
            {
                new { Id = 1, EmployeeName = "John Doe", EmployeeSalary = 50000, EmployeeAge = 30, ProfileImage = "" },
                new { Id = 2, EmployeeName = "Jane Smith", EmployeeSalary = 60000, EmployeeAge = 25, ProfileImage = "" }
            }
        };

        var jsonResponse = JsonSerializer.Serialize(mockApiResponse);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var employees = await _employeeService.GetEmployeesAsync();

        // Assert
        Assert.NotNull(employees);
        Assert.Equal(2, employees.Count());
        Assert.Equal("John Doe", employees.First().EmployeeName);
    }

    [Fact]
    public async Task GetEmployeesAsync_ShouldReturnEmptyList_WhenApiResponseIsNull()
    {
        // Arrange
        var jsonResponse = JsonSerializer.Serialize(new { Data = (object)null });

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var employees = await _employeeService.GetEmployeesAsync();

        // Assert
        Assert.NotNull(employees);
        Assert.Empty(employees);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldReturnEmployee_WhenEmployeeExists()
    {
        // Arrange
        var mockApiResponse = new
        {
            Data = new[]
            {
                new { Id = 1, EmployeeName = "John Doe", EmployeeSalary = 50000, EmployeeAge = 30, ProfileImage = "" }
            }
        };

        var jsonResponse = JsonSerializer.Serialize(mockApiResponse);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var employee = await _employeeService.GetEmployeeByIdAsync(1);

        // Assert
        Assert.NotNull(employee);
        Assert.Equal(1, employee.Id);
        Assert.Equal("John Doe", employee.EmployeeName);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldReturnNull_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var mockApiResponse = new
        {
            Data = new[]
            {
                new { Id = 1, EmployeeName = "John Doe", EmployeeSalary = 50000, EmployeeAge = 30, ProfileImage = "" }
            }
        };

        var jsonResponse = JsonSerializer.Serialize(mockApiResponse);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var employee = await _employeeService.GetEmployeeByIdAsync(999);

        // Assert
        Assert.Null(employee);
    }
}
