using System.Windows;
using System.Windows.Controls;

namespace DeLong_Desktop.Pages.Products;

/// <summary>
/// Interaction logic for ProductsPage.xaml
/// </summary>
public partial class ProductsPage : Page
{
    
    public ProductsPage()
    {
        InitializeComponent();
    }

    // Kategoriyalarni yuklash
    private async Task LoadCategoriesAsync()
    {
        //try
        //{
        //    using (var context = new YourDbContext())
        //    {
        //        var categories = await context.Categories
        //            .Select(c => new { c.Id, c.Name }) // Faqat kerakli ma'lumotlar
        //            .ToListAsync();

        //        cmbCategory.ItemsSource = categories;
        //        cmbCategory.DisplayMemberPath = "Name"; // Ko'rsatiladigan nom
        //        cmbCategory.SelectedValuePath = "Id";  // Tanlangan qiymat
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show($"Error loading categories: {ex.Message}");
        //}
    }
}
