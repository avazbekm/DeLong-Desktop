using System.ComponentModel;
using System.Runtime.CompilerServices;

public class TransferItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChange([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    private decimal _quantity;
    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != value)
            {
                _quantity = value;
                OnPropertyChange(nameof(Quantity));
                OnPropertyChange(nameof(TotalAmount));
            }
        }
    }
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount => Quantity * UnitPrice;
}