using DeLong_Desktop.ApiService.DTOs.Users;

namespace DeLong_Desktop.Pages.Customers;

public static class CustomerInfo
{
    public static long CategoryId { get; set; }
    public static long CustomerId { get; set; }
    public static long UserId { get; set; }
    public static long EmployeeId { get; set; }
    public static string UserJshshir { get; set; } = string.Empty;
    public static string YurJshshir { get; set; } = string.Empty;
    public static UserResultDto User {  get; set; }
}

