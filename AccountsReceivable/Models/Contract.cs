using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountsReceivable.Models
{
    public class Contract : IDataErrorInfo, INotifyPropertyChanged
    {
        private int id;
		private string number = null!;
        private DateTime date = DateTime.Today;
        private string nomenclature = null!;
        private string company = null!;
        private bool isValid;
        private readonly Dictionary<string, string?> errorCollection = new Dictionary<string, string?>()
        {
            ["Number"] = "",
            ["Nomenclature"] = "",
            ["Company"] = "",
        };

        [Key]
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public string Number
        {
            get { return number; }
            set { number = value; OnPropertyChanged("Number"); }
        }
        public DateTime Date
		{
			get { return date; }
			set { date = value; }
		}
		public string Nomenclature
		{
			get { return nomenclature; }
			set { nomenclature = value; OnPropertyChanged("Nomenclature"); }
		}
		public string Company
		{
			get { return company; }
			set { company = value; OnPropertyChanged("Company"); }
		}
        public bool IsValid
        {
            get { return isValid; }
        }

        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case "Number":
                        if (string.IsNullOrWhiteSpace(Number))
                        {
                            errorCollection["Number"] = "Необходимо задать номер договора";
                        }
                        else
                        {
                            errorCollection["Number"] = null;
                        }
                        break;
                    case "Nomenclature":
                        if (string.IsNullOrWhiteSpace(Nomenclature))
                        {
                            errorCollection["Nomenclature"] = "Сообщение";
                        }
                        else
                        {
                            errorCollection["Nomenclature"] = null;
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
