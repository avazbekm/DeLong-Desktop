﻿namespace DeLong_Desktop.ApiService.Models.Commons;

public class Response<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; }
}
