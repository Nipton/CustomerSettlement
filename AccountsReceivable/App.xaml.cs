using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Data.Repositories;
using AccountsReceivable.Services;
using AccountsReceivable.View;
using AccountsReceivable.ViewModels;
using AccountsReceivable.ViewModels.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AccountsReceivable
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var context = new ApplicationContext())
            {
                context.Database.EnsureCreated();
                context.Companies.Any();
            }
            var services = new ServiceCollection();

            services.AddDbContextFactory<ApplicationContext>(option => option.UseSqlite("Data Source=accounts.db"));
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddSingleton<IViewModelFactory, ViewModelFactory>();
            services.AddSingleton<IDialogService, DialogService>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<OrganizationViewModel>();
            services.AddSingleton<ReportViewModel>();

            services.AddTransient<OrganizationView>();
            services.AddTransient<CompanyEditViewModel>();           
            services.AddTransient<CompanyEditView>();

            Services = services.BuildServiceProvider();
            
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
