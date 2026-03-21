using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Interfaces
{
    internal interface ILoadable
    {
        Task LoadAsync();
    }
}
