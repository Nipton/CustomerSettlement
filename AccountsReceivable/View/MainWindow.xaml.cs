using AccountsReceivable.Services;
using AccountsReceivable.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccountsReceivable
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using(ApplicationContext db = new ApplicationContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}
