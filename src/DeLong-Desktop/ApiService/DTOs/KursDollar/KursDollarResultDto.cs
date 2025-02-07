namespace DeLong_Desktop.ApiService.DTOs.KursDollar;

public class KursDollarResultDto
{
    public long Id { get; set; }
    public decimal SellingDollar { get; set; }
    public decimal AdmissionDollar { get; set; }
    public string TodayDate { get; set; } = string.Empty;
}