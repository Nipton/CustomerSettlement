using AccountsReceivable.Data.Interfaces;
using AccountsReceivable.Exceptions;
using AccountsReceivable.Interfaces;
using AccountsReceivable.Models;
using AccountsReceivable.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountsReceivable.ViewModels
{
    public class NomenclatureViewModel : ViewModelBase, ILoadable
    {
        private readonly INomenclatureRepository nomenclatureRepository;
        private readonly IDialogService dialogService;
        private bool isLoaded;
        private string unit = string.Empty;
        private string service = string.Empty;
        public ObservableCollection<Nomenclature> NomenclatureList { get; set; } = new();
        public List<string> Units { get; set; } = new();
        public string Unit
        {
            get => unit;
            set => Set(ref unit, value);
        }
        public string Service
        {
            get => service;
            set => Set(ref service, value);
        }
        public ICommand AddNewServiceCommand { get; } 
        public ICommand DeleteServiceCommand { get; }
        public ICommand RefreshDataCommand { get; }
        public NomenclatureViewModel(INomenclatureRepository nomenclatureRepository, IDialogService dialogService) 
        {
            this.nomenclatureRepository = nomenclatureRepository;
            this.dialogService = dialogService;
            AddNewServiceCommand = new AsyncRelayCommand(AddServiceAsync, _ => !string.IsNullOrWhiteSpace(Service) && !string.IsNullOrWhiteSpace(Unit));
            DeleteServiceCommand = new AsyncRelayCommand(obj => DeleteServiceAsync(obj));
            RefreshDataCommand = new AsyncRelayCommand(async _ => { LoadUnits(); await LoadNomenclaturesAsync(); });
        }
        public async Task LoadAsync()
        {
            LoadUnits();
            if (isLoaded) return;
            await LoadNomenclaturesAsync();
        }
        private void LoadUnits()
        {
            try
            {
                Units = nomenclatureRepository.GetAllUnits();
            }
            catch (Exception)
            {
                dialogService.ShowInfo("Внимание!", "Не удалось загрузить список для единиц измерения. Проверьте источник данных.");
            }
        }
        private async Task LoadNomenclaturesAsync()
        {
            NomenclatureList.Clear();
            try
            {
                var list = await nomenclatureRepository.GetAllAsync();
                foreach (var item in list)
                {
                    NomenclatureList.Add(item);
                }
                isLoaded = true;
            }
            catch (Exception)
            {
                dialogService.ShowInfo("Внимание!", "Не удалось загрузить список услуг.");
            }
        }
        public async Task AddServiceAsync()
        {
            if (string.IsNullOrWhiteSpace(Service) || string.IsNullOrWhiteSpace(Unit))
            {
                dialogService.ShowInfo("Внимание!", "Заполните поля.");
                return;
            }
            try
            {
                var service = new Nomenclature { Name = Service, Unit = Unit };
                await nomenclatureRepository.AddAsync(service);
                NomenclatureList.Add(service);
                Unit = string.Empty;
                Service = string.Empty;
            }
            catch
            {
                dialogService.ShowError("Ошибка!", "Произошла ошибка во время сохранения.");
            }
        }
        public async Task DeleteServiceAsync(object? parameter)
        {
            if (!dialogService.ShowConfirmation("Удаление", "Вы действительно хотите удалить выбранную услугу? Отменить действие будет невозможно."))
                return;
            Nomenclature? serviceToDelete = parameter as Nomenclature;
            if (serviceToDelete == null) 
            {
                dialogService.ShowError("Ошибка!", "Произошла ошибка во время удаления.");
                return;
            }
            try
            {
                await nomenclatureRepository.DeleteAsync(serviceToDelete);
                NomenclatureList.Remove(serviceToDelete);
            }
            catch (DeleteRestrictedException ex)
            {
                dialogService.ShowError("Ошибка!", ex.Message);
            }
            catch
            {
                dialogService.ShowError("Ошибка!", "Произошла ошибка во время удаления.");
            }
        }
    }
}
