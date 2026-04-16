using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Data
{
    public class DataHub : IDataHub
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IRepository<Contract> contractRepository;
        private readonly INomenclatureRepository nomenclatureRepository;
        public ICompanyRepository CompanyRepository => companyRepository;
        public IRepository<Contract> ContractRepository => contractRepository;
        public INomenclatureRepository NomenclatureRepository => nomenclatureRepository;
        public DataHub(ICompanyRepository companyRepository, IRepository<Contract> contractRepository, INomenclatureRepository nomenclatureRepository) 
        {
            this.companyRepository = companyRepository;
            this.contractRepository = contractRepository;
            this.nomenclatureRepository = nomenclatureRepository;
        }
    }
}
