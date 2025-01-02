﻿namespace DeLong_Desktop.ApiService.DTOs.Category;

public class CategoryUpdateDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}