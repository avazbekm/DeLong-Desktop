namespace DeLong_Desktop.ApiService.DTOs.Branchs;

public class BranchUpdateDto
{
    public long Id { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}