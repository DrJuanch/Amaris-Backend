using System.Net.Http;
using System.Text.Json;

public class EmployeeService : IEmployeeService
{
    private readonly HttpClient _httpClient;

    public EmployeeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync()
    {
        var response = await _httpClient.GetAsync("https://dummy.restapiexample.com/api/v1/employees");
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var employees = apiResponse?.Data?.Select(dto => new Employee
        {
            Id = dto.Id,
            EmployeeName = dto.EmployeeName,
            EmployeeSalary = dto.EmployeeSalary,
            EmployeeAge = dto.EmployeeAge,
            ProfileImage = dto.ProfileImage
        }).ToList();

        return employees ?? new List<Employee>();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        var employees = await GetEmployeesAsync();
        return employees.FirstOrDefault(e => e.Id == id);
    }
}

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
}
