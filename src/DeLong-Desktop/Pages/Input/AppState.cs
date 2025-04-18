using DeLong_Desktop.Pages.Input;
using DeLong_Desktop.Pages.Products; // Taxmin: ProductsPage uchun
using DeLong_Desktop.Pages.InputHistories; // HistoryPage uchun

namespace DeLong_Desktop;

public static class AppState
{
    public static InputPage CurrentInputPage { get; set; }
    public static ProductsPage CurrentProductsPage { get; set; } 
    public static HistoryPage CurrentHistoryPage { get; set; } // Qo‘shildi
}