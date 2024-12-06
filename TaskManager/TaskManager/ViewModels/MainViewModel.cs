using System.Collections.ObjectModel;
using System.ComponentModel;
using TaskManager.Services;

namespace TaskManager.ViewModels;

public class MainViewModel
{

	public ObservableCollection<ITabViewModel> Tabs { get; }

	public MainViewModel()
	{
		var processesViewModel = new ProcessViewModel(new MessageBoxErrorService());
		var tasksViewModel = new TaskViewModel();

		Tabs = [processesViewModel, tasksViewModel];
	}

}