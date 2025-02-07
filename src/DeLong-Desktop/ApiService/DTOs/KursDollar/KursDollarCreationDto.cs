namespace DeLong_Desktop.ApiService.DTOs.KursDollar;

public class KursDollarCreationDto
{
    public decimal SellingDollar { get; set; }
    public decimal AdmissionDollar { get; set; }
    public string TodayDate { get; set; } = string.Empty;
}
