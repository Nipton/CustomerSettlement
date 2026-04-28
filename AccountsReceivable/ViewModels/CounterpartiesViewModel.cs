using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Exceptions;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.Models.Enums;
using AccountsReceivable.ViewModels.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class CounterpartiesViewModel : ViewModelBase, ILoadable
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IDialogService dialogService;
        private bool isLoaded;
        private string searchTerm = string.Empty;
        public ObservableCollection<Company> Companies { get; } = new();
        public ICollectionView CompaniesView { get; }
        public IList SelectedItems { get; set; } = new List<Company>();
        public string SearchTerm
        {
            get => searchTerm; 
            set { searchTerm = value; CompaniesView.Refresh(); }
        }
        public ICommand AddCompanyCommand { get; }
        public ICommand EditCompanyCommand { get; }
        public ICommand RemoveCompaniesCommand { get; }
        public ICommand RefreshCompaniesCommand { get; }

        public CounterpartiesViewModel(ICompanyRepository companyRepository, IDialogService dialogService)
        {
            this.companyRepository = companyRepository;
            this.dialogService = dialogService;
            CompaniesView = CollectionViewSource.GetDefaultView(Companies);
            CompaniesView.Filter = FilterCompany;
            AddCompanyCommand = new AsyncRelayCommand(AddCounterparty);
            EditCompanyCommand = new AsyncRelayCommand(EditCounterparty, _ => SelectedItems.Count == 1);
            RemoveCompaniesCommand = new AsyncRelayCommand(RemoveCounterpaties, _ => SelectedItems.Count != 0);
            RefreshCompaniesCommand = new AsyncRelayCommand(LoadCompaniesAsync);
        }       
        public async Task LoadAsync()
        {
            if (isLoaded) return;
            await LoadCompaniesAsync();
            isLoaded = true;
        }
        private async Task LoadCompaniesAsync()
        {
            try
            {
                Companies.Clear();
                var companyList = await companyRepository.GetAllCompaniesAsync();
                foreach (var company in companyList)
                {
                    Companies.Add(company);
                }   
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка!", "Не удалось загрузить список контрагентов.");
            }
        }
        private bool FilterCompany(object obj)
        {
            if (obj is not Company company)
                return false;
            if(string.IsNullOrWhiteSpace(SearchTerm)) 
                return true;
            if (company.Name != null && company.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (company.ShortName != null && company.ShortName.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (company.Inn != null && company.Inn.Contains(SearchTerm))
                return true;
            return false;
        }
        public async Task AddCounterparty()
        {
            var newCompany = new Company();
            try
            {
                var result =  await dialogService.ShowWindowAsync(DialogType.CompanyEditor, newCompany);
                if (result)
                    Companies.Add(newCompany);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка!", "Критическая ошибка.");
            }           
        }
        public async Task EditCounterparty()
        {
            if (SelectedItems.Count != 1)
            {
                dialogService.ShowInfo("Внимание!", "Для редактирования необходимо выделить только один элемент.");
                return;
            }
            var company = SelectedItems.Cast<Company>().First();
            try
            {
                await dialogService.ShowWindowAsync(DialogType.CompanyEditor, company);
            }
            catch (Exception)
            {
                dialogService.ShowError("Ошибка!", "Критическая ошибка.");
            }
            CompaniesView.Refresh();
        }
        public async Task RemoveCounterpaties()
        {
            if (SelectedItems.Count == 0)
            {
                dialogService.ShowInfo("Внимание!", "Для удаления необходимо выделить хотя бы один элемент.");
                return;
            }
            if (!dialogService.ShowConfirmation("Внимание!", $"Вы действительно хотите удалить выделенные элементы в количестве {SelectedItems.Count} шт.? Отменить действие будет невозможно."))
                return;
            try
            {
                var selected = SelectedItems.Cast<Company>().ToList();
                await companyRepository.DeleteCompaniesAsync(selected);
                foreach (var item in selected)
                {
                    Companies.Remove(item);
                }
            }
            catch (DeleteRestrictedException ex)
            {
                dialogService.ShowError("Ошибка!", ex.Message);
            }
            catch
            {
                dialogService.ShowError("Ошибка!", "Не удалось удалить объекты.");
            }           
        }
    }
}