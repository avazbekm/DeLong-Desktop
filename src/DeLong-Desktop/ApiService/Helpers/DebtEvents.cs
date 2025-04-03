namespace DeLong_Desktop.ApiService.Helpers;

public static class DebtEvents
{
    public static event EventHandler DebtUpdated;

    public static void RaiseDebtUpdated()
    {
        DebtUpdated?.Invoke(null, EventArgs.Empty);
    }
}