public class CurrencyData
{
    public Dictionary<string, Currency> Valute { get; set; } = new Dictionary<string, Currency>(); // Установка значения по умолчанию
    public Dictionary<string, Currency> Nominal { get; set; } = new Dictionary<string, Currency>();
}

