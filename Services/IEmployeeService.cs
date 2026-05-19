using rosa_testovoye.Models;

namespace rosa_testovoye.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeDto>> GetAllAsync();

    Task<EmployeeDto?> GetByIdAsync(int id);

    Task<EmployeeDto> CreateAsync(CreateEmployeeInput input);

    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeInput input);

    Task DeleteAsync(int id);
}
