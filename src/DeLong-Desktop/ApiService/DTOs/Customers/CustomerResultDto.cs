using DeLong_Desktop.ApiService.DTOs.Users;
using System.Text.Json.Serialization;

namespace DeLong_Desktop.ApiService.DTOs.Customers;

public class CustomerResultDto
{
    public long Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public string ManagerPhone { get; set; } = string.Empty;
    public string? MFO { get; set; } = string.Empty;
    public int? INN { get; set; }
    public string? BankAccount { get; set; } = string.Empty;
    public string? BankName { get; set; } = string.Empty;
    public string? OKONX { get; set; }
    public string? YurAddress { get; set; } = string.Empty;
    public string? EmployeeName { get; set; } = string.Empty;
    public string? EmployeePhone { get; set; } = string.Empty;
    public long BranchId { get; set; }
    public long CreatedBy { get; set; }
}
