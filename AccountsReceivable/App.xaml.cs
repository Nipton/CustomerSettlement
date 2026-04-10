using AccountsReceivable.Data;
using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Data.Repositories;
using AccountsReceivable.Models;
using AccountsReceivable.View;
using AccountsReceivable.ViewModels;
using AccountsReceivable.ViewModels.Services;
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
        public IServiceProvider Services { get; set; } = null!;
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
            services.AddTransient<IRepository<Category>, CategoryRepository>();
            services.AddTransient<INomenclatureRepository, NomenclatureRepository>();
            services.AddSingleton<IViewModelFactory, ViewModelFactory>();
            services.AddSingleton<IDialogService, DialogService>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();                     
            services.AddSingleton<ReportViewModel>();
            services.AddTransient<ReportView>();
            services.AddSingleton<OrganizationViewModel>();
            services.AddTransient<OrganizationView>();
            services.AddTransient<CompanyEditViewModel>();           
            services.AddTransient<CompanyEditView>();
            services.AddSingleton<CounterpartiesViewModel>();
            services.AddTransient<CounterpartiesView>();
            services.AddSingleton<NomenclatureViewModel>();
            services.AddTransient<NomenclatureView>();

            Services = services.BuildServiceProvider();
            
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
