using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Exceptions;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.Services;
using AccountsReceivable.ViewModels.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class CompanyEditViewModel : ViewModelBase, ILoadable, IDataErrorInfo
    {
        private const string FORMAT_ERROR = "Неверный формат";
        private Company editedСompany;
        private Company originalСompany;
        private readonly ICompanyRepository companyRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IDialogService dialogService;
        private IEnumerable<Category> categoryList = new List<Category>();
        private readonly bool isMainCompany;
        public bool IsShowCategory { get => !isMainCompany; }
        public string Header { get => isMainCompany ? "Организация" : "Контрагент"; }
        public IEnumerable<Category> CategoryList
        {
            get => categoryList;
            set => Set(ref categoryList, value);
        }
        #region Свойства Company
        public string Name
        {
            get => editedСompany.Name;
            set => SetProperty(v => editedСompany.Name = v, value); 
        }
        public string ShortName
        {
            get => editedСompany.ShortName;
            set => SetProperty(v => editedСompany.ShortName = v, value);
        }
        public string LegalAddress
        {
            get => editedСompany.LegalAddress;
            set => SetProperty(v => editedСompany.LegalAddress = v, value);
        }
        public string ActualAddress
        {
            get => editedСompany.ActualAddress;
            set => SetProperty(v => editedСompany.ActualAddress = v, value);
        }
        public string Inn
        {
            get => editedСompany.Inn;
            set => SetProperty(v => editedСompany.Inn = v, value);
        }
        public string Kpp
        {
            get => editedСompany.Kpp;
            set => SetProperty(v => editedСompany.Kpp = v, value);
        }
        public string Ogrn
        {
            get => editedСompany.Ogrn;
            set => SetProperty(v => editedСompany.Ogrn = v, value);
        }
        public string Bank
        {
            get => editedСompany.Bank;
            set => SetProperty(v => editedСompany.Bank = v, value);
        }
        public string Rs
        {
            get => editedСompany.Rs;
            set => SetProperty(v => editedСompany.Rs = v, value);
        }
        public string Ks
        {
            get => editedСompany.Ks;
            set => SetProperty(v => editedСompany.Ks = v, value);
        }
        public string Bik
        {
            get => editedСompany.Bik;
            set => SetProperty(v => editedСompany.Bik = v, value);
        }
        public string Phone
        {
            get => editedСompany.Phone;
            set => SetProperty(v => editedСompany.Phone = v, value);
        }
        public string Position
        {
            get => editedСompany.Position;
            set => SetProperty(v => editedСompany.Position = v, value);
        }
        public string DirectorFullName
        {
            get => editedСompany.DirectorFullName;
            set => SetProperty(v => editedСompany.DirectorFullName = v, value);
        }
        public Category? Category
        {
            get => editedСompany.Category;
            set => SetProperty(v => editedСompany.Category = v, value);
        }
        public int? CategoryId
        {
            get => editedСompany.CategoryId;
            set => SetProperty(v => editedСompany.CategoryId = v, value);
        }
        #endregion
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadEditViewCommand { get; }
        
        public CompanyEditViewModel(Company originalСompany, ICompanyRepository companyRepository, IRepository<Category> categoryRepository, IDialogService dialogService) 
        {
            CancelCommand = new RelayCommand(_ => Cancel());
            SaveCommand = new AsyncRelayCommand(Save, _ => IsValid);
            LoadEditViewCommand = new AsyncRelayCommand(LoadAsync);

            this.companyRepository = companyRepository;
            this.categoryRepository = categoryRepository;
            this.dialogService = dialogService;
            this.originalСompany = originalСompany;
            editedСompany = originalСompany.Clone() as Company ?? throw new CloneException("Не удалось клонировать компанию");
            editedСompany.Category = null;
            if(originalСompany.Id == Constants.OWN_COMPANY_ID) 
                isMainCompany = true;
        }
        private void Cancel()
        {
            dialogService.CloseWindow(this, false);
        }
        private async Task Save()
        {
            if (!IsValid)
            {
                dialogService.ShowInfo("Ошибка", "Исправьте ошибки перед сохранением");
                return;
            }
            try
            {
                if (editedСompany.Id == 0)
                    await companyRepository.AddCompanyAsync(editedСompany);
                else
                    await companyRepository.UpdateCompanyAsync(editedСompany);
                originalСompany.CopyFrom(editedСompany);
                originalСompany.Category = categoryList.FirstOrDefault(c => c.Id == originalСompany.CategoryId);
                dialogService.CloseWindow(this, true);
            }
            catch 
            {
                dialogService.ShowError("Ошибка", "Произошла ошибка во время сохранения.");
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                CategoryList = await categoryRepository.GetAllAsync();
            }
            catch
            {
                dialogService.ShowError("Ошибка", "Не удалось загрузить список категорий.");
            }
        }
        #region Валидация
        public string Error => null!;
        private Dictionary<string, string?> errorCollection = new() {[nameof(Name)] = null, [nameof(ShortName)] = null};
        public bool IsValid => errorCollection.Values.All(x => x == null);
        #nullable disable
        public string this[string columnName]
        {
            get
            {
                string error = columnName switch
                {
                    nameof(Name) => ValidateName(Name),
                    nameof(ShortName) => ValidateShortName(ShortName),
                    nameof(Inn) => ValidateDigits(Inn, new[] { 10, 12 }),
                    nameof(Kpp) => ValidateDigits(Kpp, 9),
                    nameof(Ogrn) => ValidateDigits(Ogrn, 13),
                    nameof(Bik) => ValidateDigits(Bik, 9),
                    nameof(Rs) => ValidateDigits(Rs, 20),
                    nameof(Ks) => ValidateDigits(Ks, 20),
                    nameof(Phone) => ValidatePhone(Phone),
                    _ => null,
                };
                errorCollection[columnName] = error;
                OnPropertyChanged(nameof(IsValid));
                return error;
            }
        }
        #nullable restore
        private static string? ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return  "Название не может быть пустым";
            else if (name.Length < 5)
                return "Слишком короткое название";
            else
                return null;
        }
        private static string? ValidateShortName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Это обязательное поле";
            if (name.Length < 3)
                return "Слишком короткое название";
            return null;
        }
        private static string? ValidateDigits(string? value, params int[] lengths)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            if (!value.All(char.IsDigit) || !lengths.Contains(value.Length))
                return FORMAT_ERROR;
            return null;
        }
        private static string? ValidatePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return null;
            char[] allowed = { '(', ')', ' ', '+', '-' };
            if (phone.Any(c => !char.IsDigit(c) && !allowed.Contains(c)))
                return FORMAT_ERROR;
            return null;
        }
        #endregion
    }
}
