using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Models;
using Flowers.Services;

[QueryProperty(nameof(Bouquet), "Bouquet")]
public partial class BouquetFormViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private Bouquet bouquet;

    public ObservableCollection<KeyValuePair<string, int>> EditableFlowers { get; set; }

    public ICommand AddFlowerCommand { get; }
    public ICommand RemoveFlowerCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SelectImageCommand { get; }

    [ObservableProperty]
    public string pageTitle;

    public BouquetFormViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        EditableFlowers = new ObservableCollection<KeyValuePair<string, int>>();



        AddFlowerCommand = new RelayCommand(OnAddFlower);
        RemoveFlowerCommand = new RelayCommand<KeyValuePair<string, int>>(OnRemoveFlower);
        SaveCommand = new RelayCommand(OnSave);
        CancelCommand = new RelayCommand(OnCancel);
        SelectImageCommand = new RelayCommand(OnSelectImage);
    }

    // Метод вызывается автоматически при установке свойства "Bouquet"
    partial void OnBouquetChanged(Bouquet value)
    {
        if (value != null)
        {
            PageTitle = "Редактирование букета";
            EditableFlowers = new ObservableCollection<KeyValuePair<string, int>>(value.Flowers);
        }
        else
        {
            PageTitle = "Добавление букета";
            EditableFlowers = new ObservableCollection<KeyValuePair<string, int>>();
        }

        // Уведомляем UI об изменениях коллекции
        OnPropertyChanged(nameof(EditableFlowers));
    }

    private void OnAddFlower()
    {
        EditableFlowers.Add(new KeyValuePair<string, int>("Новый цветок", 1));
    }

    private void OnRemoveFlower(KeyValuePair<string, int> flower)
    {
        EditableFlowers.Remove(flower);
    }

    private async void OnSave()
    {
        Bouquet.Flowers = EditableFlowers.ToDictionary(x => x.Key, x => x.Value);

        if (Bouquet.Id == 0)
        {
            await _databaseService.AddBouquetAsync(Bouquet);
        }
        else
        {
            await _databaseService.UpdateBouquetAsync(Bouquet);
        }

    }

    private void OnCancel()
    {
        
    }

    private async void OnSelectImage()
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Выберите изображение букета",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
        {
            using var stream = await result.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            Bouquet.Icon = memoryStream.ToArray();
        }
    }
}
