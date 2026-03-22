using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Exceptions;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.Services;
using AccountsReceivable.View;
using AccountsReceivable.ViewModels.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class OrganizationViewModel : ViewModelBase, ILoadable
    {
        private readonly ICompanyRepository repository;
        private readonly IDialogService dialogService;
        private bool isLoaded;
        private Company? organization;
        public Company? Organization
        {
            get => organization;
            set => Set(ref organization, value);
        }
        public ICommand LoadOrganizationCommand { get; }
        public ICommand EditOrganizationCommand { get; }
        public OrganizationViewModel(ICompanyRepository companyRepository, IDialogService dialogService)
        {
            repository = companyRepository;
            this.dialogService = dialogService;
            LoadOrganizationCommand = new RelayCommand(async (_) => await LoadAsync());
            EditOrganizationCommand = new RelayCommand(_ => EditOrganization());
        }

        private void EditOrganization()
        {
            if (Organization == null) return;
            try
            {
                dialogService.ShowWindow<CompanyEditView, CompanyEditViewModel>(Organization);

                OnPropertyChanged(nameof(Organization));
            }
            catch (CloneException ex)
            {
                dialogService.ShowError("Ошибка!", ex.Message);
            }
            catch(Exception)
            {
                dialogService.ShowError("Ошибка!", "Критическая ошибка.");
            }
        }

        public async Task LoadAsync()
        {
            if (isLoaded) return;
            try
            {
                Organization = await repository.GetCompanyAsync(Constants.OWN_COMPANY_ID);
                isLoaded = true;
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка!", "Ошибка загрузки компании.");
            }
        }
    }
}
