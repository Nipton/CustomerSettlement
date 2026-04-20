using System.Threading.Tasks;

namespace AccountsReceivable.Interfaces
{
    internal interface ILoadable
    {
        Task LoadAsync();
    }
}
