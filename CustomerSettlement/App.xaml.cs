using CustomerSettlement.Data;
using CustomerSettlement.Data.Interfaces;
using CustomerSettlement.Data.Repositories;
using CustomerSettlement.Helpers;
using CustomerSettlement.Interfaces;
using CustomerSettlement.Services;
using CustomerSettlement.View;
using CustomerSettlement.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CustomerSettlement
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; set; } = null!;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddDbContextFactory<ApplicationContext>(option => option.UseSqlite(Constants.DATABASE_CONNECTION_STRING));   

            services.AddSingleton<IViewModelFactory, ViewModelFactory>();

            services.AddSingleton<ICompanyRepository, CompanyRepository>();
            services.AddSingleton<INomenclatureRepository, NomenclatureRepository>();
            services.AddSingleton<IContractRepository, ContractRepository>(); 
            services.AddSingleton<IAccountRepository, AccountRepository>(); 
            services.AddSingleton<ITemplateRepository, TemplateRepository>();
            services.AddTransient<IAccountEditingService, AccountEditingService>();
            
            services.AddSingleton<IDialogService, DialogService>();
            services.AddTransient<IReconciliationReportBuilder, ReconciliationReportBuilder>();
            services.AddTransient<ReconciliationDocumentService>();
            services.AddTransient<ActDocumentService>();
            services.AddTransient<InvoiceDocumentService>();
            services.AddTransient<IDocumentServiceFactory, DocumentServiceFactory>();

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
            WarmupDatabase(Services);
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        private void WarmupDatabase(IServiceProvider provider)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var factory = provider.GetRequiredService<IDbContextFactory<ApplicationContext>>();

                    using var context = await factory.CreateDbContextAsync();
                    await context.Database.MigrateAsync();
                    await context.Companies.AsNoTracking().AnyAsync();
                }
                catch (Exception)
                {
                    MessageBox.Show( $"Не удалось инициализировать базу данных","Ошибка запуска",MessageBoxButton.OK,MessageBoxImage.Error);
                    Current.Dispatcher.Invoke(() => Current.Shutdown());
                }
            });
        }
    }
}
