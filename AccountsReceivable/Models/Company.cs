using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountsReceivable.Models
{
    [Serializable]
    public class Company : IDataErrorInfo, INotifyPropertyChanged, ICloneable
	{
        private const string FORMAT_ERROR = "Неверный формат ввода";
        private int id;
        private string name;
		private string shortName;
		private string legalAddress;
		private string actualAddress;
		private string inn;
		private string kpp;
		private string ogrn;
		private string bank;
		private string rs;
		private string bik;
		private string ks;
		private string phone;
		private string position;
		private string fullName;
		private string category;
        private bool isValid;

        [XmlIgnore]
        public Dictionary<string, string?> errorCollection = new Dictionary<string, string?>()
		{
			["Name"] = "",
			["ShortName"] = "",
		};
				
		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; OnPropertyChanged("Name"); }
		}
		public string ShortName
		{
			get { return shortName; }
			set { shortName = value; OnPropertyChanged("ShortName"); }
		}
		public string LegalAddress
		{
			get { return legalAddress; }
			set { legalAddress = value; OnPropertyChanged("LegalAddress"); }
		}
		public string ActualAddress
		{
			get { return actualAddress; }
			set { actualAddress = value; OnPropertyChanged("ActualAddress"); }
		}
		public string Inn
		{
			get { return inn; }
			set { inn = value; OnPropertyChanged("Inn"); }
		}
		public string Kpp
		{
			get { return kpp; }
			set { kpp = value; OnPropertyChanged("Kpp"); }
		}
		public string Ogrn
		{
			get { return ogrn; }
			set { ogrn = value; OnPropertyChanged("Ogrn"); }
		}
		public string Bank
		{
			get { return bank; }
			set { bank = value; OnPropertyChanged("Bank"); }
		}
		public string Rs
		{
			get { return rs; }
			set 
			{
                rs = value;
                OnPropertyChanged("Rs");
			}
		}
		public string Bik
		{
			get { return bik; }
			set
			{ 
				bik = value;
                OnPropertyChanged("Bik");
            }
		}
		public string Phone
		{
			get { return phone; }
			set { phone = value; OnPropertyChanged("Phone"); }
		}
		public string Position
		{
			get { return position; }
			set { position = value; OnPropertyChanged("Position"); }
		}
		public string DirectorFullName
		{
			get { return fullName; }
			set { fullName = value; OnPropertyChanged("DirectorFullName"); }
		}
        public string Ks
        {
            get { return ks; }
            set { ks = value; OnPropertyChanged("Ks"); }
        }
        public bool IsValid
        {
            get { return isValid; }
        }
        public string Category
        {
            get { return category; }
            set { category = value; }
        }
        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public Company()
        {
        }
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Rs":
                        if (!string.IsNullOrEmpty(Rs))
                        {
							if (Rs.Length != 20 || !Rs.All(x => char.IsNumber(x)))
							{
								errorCollection["Rs"] = FORMAT_ERROR;
							}
                            else
                            {
                                errorCollection["Rs"] = null;
                            }
                        }
						else
						{
							errorCollection["Rs"] = null;
                        }
                        break;
					case "Name":
						if(string.IsNullOrWhiteSpace(Name))
						{
                            errorCollection["Name"] = "Название не может быть пустым";
						}
						else if(Name.Length < 5)
						{
                            errorCollection["Name"] = "Слишком короткое название";
						}
						else
						{
							errorCollection["Name"] = null;
                        }
						break;
                    case "ShortName":
                        if (string.IsNullOrWhiteSpace(ShortName))
                        {
                            errorCollection["ShortName"] = "Это обязательное поле";
                        }
                        else if (ShortName.Length < 3)
                        {
                            errorCollection["ShortName"] = "Слишком короткое название";
                        }
                        else
                        {
                            errorCollection["ShortName"] = null;
                        }
                        break;
                    case "Inn":
                        if (!string.IsNullOrEmpty(Inn))
                        {
                            if (Inn.Length != 12 && Inn.Length != 10 || !Inn.All(x => char.IsNumber(x)))
                            {
                                errorCollection["Inn"] = FORMAT_ERROR;
                            }
                            else
                            {
                                errorCollection["Inn"] = null;
                            }
                        }
						else
						{
							errorCollection["Inn"] = null;
                        }
						break;
					case "Kpp":
                        if (!string.IsNullOrEmpty(Kpp))
                        {
                            if (Kpp.Length != 9 || !Kpp.All(x => char.IsNumber(x)))
                            {
                                errorCollection["Kpp"] = FORMAT_ERROR;
                            }
                            else
                            {
                                errorCollection["Kpp"] = null;
                            }
                        }
						else
						{
							errorCollection["Kpp"] = null;
                        }
                        break;
					case "Ogrn":
                        if (!string.IsNullOrEmpty(Ogrn))
                        {
                            if (Ogrn.Length != 13 || !Ogrn.All(x => char.IsNumber(x)))
                            {
                                errorCollection["Ogrn"] = FORMAT_ERROR;
                            }
                            else
                            {
                                errorCollection["Ogrn"] = null;
                            }
                        }
						else
						{
							errorCollection["Ogrn"] = null;
                        }
                        break;
					case "Bik":
                        if (!string.IsNullOrEmpty(Bik))
                        {
                            if (Bik.Length != 9 || !Bik.All(x => char.IsNumber(x)))
                            {
                                errorCollection["Bik"] = FORMAT_ERROR;
                            }
                            else
                            {
                                errorCollection["Bik"] = null;
                            }
                        }
						else
						{
							errorCollection["Bik"] = null;
                        }
                        break;
					case "Ks":
                        if (!string.IsNullOrEmpty(Ks))
                        {
                            if (Ks.Length != 20 || !Ks.All(x => char.IsNumber(x)))
                            {
                                errorCollection["Ks"] = FORMAT_ERROR;
                            }
                            else
                            {
                                errorCollection["Ks"] = null;
                            }
                        }
						else
						{
							errorCollection["Ks"] = null;
                        }
                        break;
					case "Phone":
						if(!string.IsNullOrEmpty(Phone))
						{
							char[] ch = new char[] { '(', ')', ' ', '+', '-' };
							for (int i = 0; i < Phone.Length; i++)
							{
								if (!char.IsNumber(Phone[i]) && !ch.Contains(Phone[i]))
								{
                                    errorCollection["Phone"] = FORMAT_ERROR;
								}
                                else
                                {
                                    errorCollection["Phone"] = null;
                                }
                            }
						}
						else
						{
							errorCollection["Phone"] = null;
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
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
		public object Clone() => MemberwiseClone();
    }
}
