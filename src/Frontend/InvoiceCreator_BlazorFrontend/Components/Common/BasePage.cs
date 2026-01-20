using Microsoft.AspNetCore.Components;

namespace InvoiceCreator_BlazorFrontend.Components.Common
{
    public abstract class BasePage : ComponentBase
    {
        protected bool IsBusy { get; set; }
        protected string Fehlermeldung { get; set; }

        protected async Task TryCatchAsync(Func<Task> action)
        {
            IsBusy = true;
            try { await action(); }
            catch (Exception ex) { Fehlermeldung = ex.Message; }
            finally { IsBusy = false; }
        }
    }
}
