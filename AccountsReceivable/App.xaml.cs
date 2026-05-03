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
using System.Windows.Controls;
using System.Windows.Input;

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
            services.AddSingleton<IViewModelFactory, ViewModelFactory>();

            services.AddSingleton<ICompanyRepository, CompanyRepository>();
            services.AddSingleton<INomenclatureRepository, NomenclatureRepository>();
            services.AddSingleton<IContractRepository, ContractRepository>(); 
            services.AddSingleton<IAccountRepository, AccountRepository>(); 
            services.AddSingleton<ITemplateRepository, TemplateRepository>();
            services.AddTransient<IAccountEditingService, AccountEditingService>();
            
            services.AddSingleton<IDialogService, DialogService>();
            services.AddTransient<IReconciliationReportBuilder, ReconciliationReportBuilder>();
            services.AddTransient<IDocumentService<ReconciliationReport>, ReconciliationDocumentService>();
            services.AddTransient<IDocumentService<AccountHeader>, ActDocumentService>();

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
            services.AddTransient<AccountEditorViewModel>();
            services.AddTransient<PaymentView>();
            services.AddTransient<PaymentViewModel>();
            services.AddSingleton<ReconciliationReportViewModel>();
            services.AddTransient<ReconciliationReportView>();
            services.AddTransient<PrintPreviewViewModel>();
            services.AddTransient<PrintPreviewView>();

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }        
    }
}
