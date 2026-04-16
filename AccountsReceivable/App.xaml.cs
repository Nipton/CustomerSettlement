using AccountsReceivable.Data;
using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Data.Repositories;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.Services;
using AccountsReceivable.View;
using AccountsReceivable.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Companies.Any();
            }
            var services = new ServiceCollection();

            services.AddDbContextFactory<ApplicationContext>(option => option.UseSqlite("Data Source=accounts.db"));
            services.AddSingleton<ICompanyRepository, CompanyRepository>();
            services.AddSingleton<IRepository<Category>, CategoryRepository>();
            services.AddSingleton<INomenclatureRepository, NomenclatureRepository>();
            services.AddSingleton<IRepository<Contract>, ContractRepository>(); 
            services.AddSingleton<IViewModelFactory, ViewModelFactory>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IDataHub, DataHub>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();                     
            services.AddSingleton<ReportViewModel>();
            services.AddTransient<ReportView>();
            services.AddSingleton<OrganizationViewModel>();
            services.AddTransient<OrganizationView>();
            services.AddTransient<CompanyEditorViewModel>();           
            services.AddTransient<CompanyEditorView>();
            services.AddSingleton<CounterpartiesViewModel>();
            services.AddTransient<CounterpartiesView>();
            services.AddSingleton<NomenclatureViewModel>();
            services.AddTransient<NomenclatureView>();
            services.AddSingleton<ContractViewModel>();
            services.AddTransient<ContractView>();
            services.AddSingleton<AccountsJournalViewModel>();
            services.AddTransient<AccountsView>();
            services.AddTransient<AccountEditorView>();

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
