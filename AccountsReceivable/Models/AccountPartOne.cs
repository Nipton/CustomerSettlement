using AccountsReceivable.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Models
{
    [Index("ID")]
    public class AccountPartOne : IDataErrorInfo, INotifyPropertyChanged
    {
		private int id;
        private Contract? contract;
		private string company = null!;
		private DateTime date = DateTime.Today;
        private double sum;
        private double? payment;
        private bool? paymentStatus;
        private bool? actStatus;
        private bool isValid;

        [NotMapped]
        public Dictionary<string, string?> errorCollection { get; set; } = new Dictionary<string, string?>()
        {
            ["Contract"] = "",
            ["Company"] = "",
        };
        public List<AccountPartTwo> AccountsList { get; set; } = new();
        public List<Payment> PaymentsList { get; set; } = new();
        public int? ContractID { get; set; }
        public int ID
		{	
			get { return id; }
			set { id = value; }
		}
		public Contract? Contract
        {
			get { return contract; }
			set { contract = value; OnPropertyChanged("Contract"); }
		}
        public string Company
        {
            get { return company; }
            set { company = value; OnPropertyChanged("Company"); }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public bool IsValid
        {
            get { return isValid; }
        }
        public double Sum
        {
            get { return sum; }
            set { sum = value; }
        }
        public double? Payment
        {
            get { return payment; }
            set { payment = value; }
        }
        public bool? PaymentStatus
        {
            get { return paymentStatus; }
            set { paymentStatus = value;}
        }

        public bool? ActStatus
        {
            get { return actStatus; }

            set { actStatus = value; }
        }

        //public AccountPartOne() { }
        //public AccountPartOne(AccountPartOne newAccOne)
        //{
        //    ContractID = newAccOne.ContractID;
        //    Contract = newAccOne.Contract;
        //    Company = newAccOne.Company;
        //    ID = newAccOne.ID;
        //    Sum = newAccOne.Sum;
        //    errorCollection = new Dictionary<string, string?>();
        //}
        public string Error => throw new NotImplementedException();

        

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Contract":
                        if (Contract == null)
                        {
                            errorCollection["Contract"] = "Сообщение";
                        }
                        else
                        {
                            errorCollection["Contract"] = null;
                        }
                        break;
                    case "Company":
                        if (string.IsNullOrWhiteSpace(Company))
                        {
                            errorCollection["Company"] = "Сообщение";
                        }
                        else
                        {
                            errorCollection["Company"] = null;
                        }
                        break;
                }
                isValid = !errorCollection.Values.Any(x => x != null);
                OnPropertyChanged("IsValid");
                return errorCollection.ContainsKey(columnName) ? errorCollection[columnName] : null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
