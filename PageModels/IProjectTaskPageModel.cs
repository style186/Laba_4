using CommunityToolkit.Mvvm.Input;
using CurrencyConverter.Models;

namespace CurrencyConverter.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}