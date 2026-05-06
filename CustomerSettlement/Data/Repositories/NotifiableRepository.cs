using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerSettlement.Data.Repositories
{
    public abstract class NotifiableRepository
    {
        public event Func<Task>? DataChanged;
        protected async Task NotifyDataChangedAsync()
        {
            if (DataChanged != null)
            {
                var handlers = DataChanged.GetInvocationList().Cast<Func<Task>>();
                var tasks = handlers.Select(async handle =>
                {
                    try
                    {
                        await handle();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Ошибка в обработчике DataChanged : {ex.Message}");
                    }
                }).ToArray();
                await Task.WhenAll(tasks);
            }
        }
    }
}
