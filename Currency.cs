public class Currency
{
    public string CharCode { get; set; } = string.Empty; // Установка значения по умолчанию
    public double Value { get; set; }
    public string Name { get; set; } = string.Empty; // Установка значения по умолчанию

    public double Nominal { get; set; }     // Установка значения по умолчанию
}
