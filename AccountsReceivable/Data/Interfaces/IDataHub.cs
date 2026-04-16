using AccountsReceivable.Models;

namespace AccountsReceivable.Data.Interfaces
{
    public interface IDataHub
    {
        ICompanyRepository CompanyRepository { get; }
        INomenclatureRepository NomenclatureRepository { get; }
        IRepository<Contract> ContractRepository { get; }
    }
}
