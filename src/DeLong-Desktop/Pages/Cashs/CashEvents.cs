namespace DeLong_Desktop.Pages.Cashs;

public static class CashEvents
{
    public static event EventHandler CashUpdated;

    public static void RaiseCashUpdated()
    {
        CashUpdated?.Invoke(null, EventArgs.Empty);
    }
}