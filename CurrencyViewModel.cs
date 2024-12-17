using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json;

public class CurrencyViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Currency> Currencies { get; set; } = new ObservableCollection<Currency>();

    private Currency fromCurrency = new Currency(); // Установка значения по умолчанию
    public Currency FromCurrency
    {
        get => fromCurrency;
        set
        {
            fromCurrency = value;
            OnPropertyChanged(nameof(FromCurrency));
        }
    }

    private Currency toCurrency = new Currency(); // Установка значения по умолчанию
    public Currency ToCurrency
    {
        get => toCurrency;
        set
        {
            toCurrency = value;
            OnPropertyChanged(nameof(ToCurrency));
        }
    }

    private double amount;
    public double Amount
    {
        get => amount;
        set
        {
            amount = value;
            OnPropertyChanged(nameof(Amount));
        }
    }

    private string message = string.Empty;
    public string Message
    {
        get => message;
        set
        {
            message = value;
            OnPropertyChanged(nameof(Message));
        }
    }

    private DateTime selectedDate = DateTime.Now;
    public DateTime SelectedDate
    {
        get => selectedDate;
        set
        {
            selectedDate = value;
            OnPropertyChanged(nameof(SelectedDate));
            LoadCurrencies(); // Загружаем валюты при изменении даты
        }
    }

    public ICommand ConvertCommand { get; }

    public CurrencyViewModel()
    {
        LoadCurrencies();
        ConvertCommand = new Command(ConvertCurrency);
    }

    public DateTime currentDate;

    private async void LoadCurrencies()
    {
        DateTime searchDate = SelectedDate; // Начинаем с выбранной даты
        bool dataFound = false;

        // Проверяем, что выбранная дата не находится в будущем
        if (searchDate > DateTime.Now)
        {
            Message = "Выберите дату, которая не находится в будущем.";
            return;
        }

        using (var httpClient = new HttpClient())
        {
            // Проверяем даты в пределах 30 дней назад
            for (int daysBack = 0; daysBack < 30; daysBack++)
            {
                currentDate = searchDate.AddDays(-daysBack); // Уменьшаем дату на daysBack

                string url = $"https://www.cbr-xml-daily.ru/archive/{currentDate.Year}/{currentDate.Month:D2}/{currentDate.Day:D2}/daily_json.js";
                Console.WriteLine($"Запрос к URL: {url}"); // Логируем URL

                try
                {
                    var response = await httpClient.GetStringAsync(url);
                    var currencyData = JsonConvert.DeserializeObject<CurrencyData>(response);

                    // Проверяем, были ли данные найдены
                    if (currencyData != null && currencyData.Valute != null && currencyData.Valute.Count > 0)
                    {
                        Currencies.Clear();
                        foreach (var currency in currencyData.Valute.Values)
                        {
                            Currencies.Add(currency);
                        }
                        Currencies.Add(new Currency() { CharCode = "RUB", Name = "Рубль", Value = 1 , Nominal = 1 });
                        // Если данные найдены, устанавливаем флаг и выводим сообщение
                        dataFound = true;
                        Message = $"Курсы валют найдены на {currentDate:yyyy-MM-dd}.";
                        break; // Выходим из цикла, если данные найдены
                    }
                    else
                    {
                        Console.WriteLine($"Данные пустые для даты: {currentDate:yyyy-MM-dd}."); // Логируем пустые данные
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Ошибка HTTP: {ex.Message}"); // Логируем ошибку
                }
                catch (Exception ex)
                {
                    Message = $"Ошибка загрузки данных: {ex.Message}";
                    return; // Выходим из метода на случай ошибки
                }
            }

            // Если данные не найдены за последние 30 дней
            if (!dataFound)
            {
                Message = "Курсы валют не найдены за последние 30 дней.";
            }
        }
    }




    private void ConvertCurrency()
    {
        if (FromCurrency != null && ToCurrency != null)
        {
            double result = (Amount * FromCurrency.Value / FromCurrency.Nominal) * ToCurrency.Nominal / ToCurrency.Value;
            //double result = (Amount * FromCurrency.Value) / ToCurrency.Value;
            Message = $"{Amount} {FromCurrency.CharCode} = {result} {ToCurrency.CharCode} (на {currentDate:yyyy-MM-dd})";
            
        }
        else
        {
            Message = "Пожалуйста, выберите валюты.";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged; // Используем ? для допуска значения NULL
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}