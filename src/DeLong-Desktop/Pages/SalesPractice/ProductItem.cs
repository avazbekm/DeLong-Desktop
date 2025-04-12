using System.ComponentModel;

public class ProductItem : INotifyPropertyChanged
{
    public long PriceId { get; set; }
    public int SerialNumber { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty;
    public long ProductId { get; set; }
    public decimal CostPrice {  get; set; } 
    public decimal BalanceAmount { get; set; }
    public string ProductSign { get; set; } = string.Empty;

    private decimal _quantity;
    public decimal Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged(nameof(Quantity));
            OnPropertyChanged(nameof(TotalPrice));
        }
    }

    public decimal TotalPrice => Price * Quantity;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}