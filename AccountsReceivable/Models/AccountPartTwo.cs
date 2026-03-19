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
    public class AccountPartTwo : INotifyPropertyChanged
    {
		private int id;
        private int number;
        private string? nomenclature;
		private string? unit;
        private double quantity = 1;
        private double price = 0;
        private double vat = 0;
        private double sum;
		private DateTime? period = null;

        [ForeignKey("Number")]
        public AccountPartOne? AccountPartOne { get; set; }
		public int ID
		{
			get { return id; }
			set { id = value; }
		}      
        public int Number
		{
			get { return number; }
			set { number = value; }
		}
        public string? Nomenclature
		{
			get { return nomenclature; }
			set { nomenclature = value; }
		}
        public string? Unit
        {
            get { return unit; }
            set { unit = value; }
        }
		public double Quantity
        {
			get { return quantity; }
			set { quantity = value; OnPropertyChanged("Quantity"); }
		}
		public double Price
		{
			get { return price; }
			set { price = value; OnPropertyChanged("Price"); }
		}		
		public double VAT
		{
			get { return vat; }
			set { vat = value; OnPropertyChanged("VAT"); }
		}
		public double Sum
		{
			get { return sum; }
			set { sum = value; OnPropertyChanged("Sum"); }
		}
		public DateTime? Period
		{
			set { period = value; }
			get { return period; }
		}
		[NotMapped]
		public double? sumWithOutVat { get; set; }
		[NotMapped]
		public double? sumVat { get; set; }

		public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
