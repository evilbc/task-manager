using System.Collections.ObjectModel;
using System.ComponentModel;
using TaskManager.Services;

namespace TaskManager.ViewModels;

public class MainViewModel
{

	public ObservableCollection<ITabViewModel> Tabs { get; }

	public MainViewModel()
	{
		var errorMessageService = new MessageBoxErrorService();
		var processesViewModel = new ProcessViewModel(errorMessageService);
		var tasksViewModel = new TaskViewModel(errorMessageService);

		Tabs = [processesViewModel, tasksViewModel];
	}

}