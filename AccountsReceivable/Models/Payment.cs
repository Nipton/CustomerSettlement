using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsReceivable.Models
{
    public class Payment
    {
        private int id;
        private string number = "";
        private double? sum = 0;
        private DateTime date = DateTime.Today;
        private int accountID;

		public int ID
		{
			get { return id; }
			set { id = value; }
		}
		public string Number
		{
			get { return number; }
			set { number = value; }
		}
		public double? Sum
		{
			get { return sum; }
			set { sum = value; }
		}
		public DateTime Date
		{
			get { return date; }
			set { date = value; }
		}
		public int AccountID
		{
			get { return accountID; }
			set { accountID = value; }
		}

        [ForeignKey("AccountID")]
        public AccountPartOne? AccountPartOne { get; set; }
    }
}
