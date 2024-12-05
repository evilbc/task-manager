using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.ViewModels
{
	public class MainViewModel
	{
		private readonly IErrorMessageService _errorMessageService;
		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private readonly ObservableCollection<ProcessModel> _processes;
		public ICollectionView Processes { get; }


		private ProcessModel? _selectedProcess;
		public ProcessModel? SelectedProcess
		{
			get => _selectedProcess;
			set
			{
				_selectedProcess = value;
				OnPropertyChanged(nameof(SelectedProcess));
				_changePriorityCommand.NotifyCanExecuteChanged();
				_killProcessCommand.NotifyCanExecuteChanged();
				if (value is { Priority: not null })
				{
					SelectedPriority = value.Priority.Value;
				}
			}
		}

		private string _filterText;
		public string FilterText
		{
			get => _filterText;
			set
			{
				_filterText = value;
				OnPropertyChanged(nameof(FilterText));
				Processes.Refresh();
			}
		}

		public List<ProcessPriorityClass> Priorities { get; }
		private ProcessPriorityClass? _selectedPriority;

		public ProcessPriorityClass? SelectedPriority
		{
			get => _selectedPriority;
			set
			{
				_selectedPriority = value;
				OnPropertyChanged(nameof(SelectedPriority));
				_changePriorityCommand.NotifyCanExecuteChanged();
			}
		}

		public ICommand RefreshCommand { get; }
		private readonly RelayCommand _killProcessCommand;
		public ICommand KillProcessCommand => _killProcessCommand;
		private readonly RelayCommand _changePriorityCommand;
		public ICommand ChangePriorityCommand => _changePriorityCommand;


		public MainViewModel(IErrorMessageService errorMessageService)
		{
			_errorMessageService = errorMessageService;
			RefreshCommand = new RelayCommand(LoadProcesses);
			_killProcessCommand = new RelayCommand(KillProcess, () => SelectedProcess != null);
			_changePriorityCommand = new RelayCommand(ChangePriority, () => SelectedProcess != null && SelectedPriority != null);

			_processes = new ObservableCollection<ProcessModel>();
			LoadProcesses();
			Processes = CollectionViewSource.GetDefaultView(_processes);
			Processes.Filter = FilterProcesses;

			Priorities = new List<ProcessPriorityClass>
			{
				ProcessPriorityClass.Idle,
				ProcessPriorityClass.BelowNormal,
				ProcessPriorityClass.Normal,
				ProcessPriorityClass.AboveNormal,
				ProcessPriorityClass.High,
				ProcessPriorityClass.RealTime
			};
			SelectedPriority = ProcessPriorityClass.Normal;

		}

		private bool FilterProcesses(object item)
		{
			if (item is ProcessModel process)
			{
				return string.IsNullOrWhiteSpace(FilterText) || process.Name.Contains(FilterText, StringComparison.CurrentCultureIgnoreCase);
			}
			return false;
		}

		private async void LoadProcesses()
		{
			// List<ProcessModel> 
			// _processes.Clear();
			// foreach (var process in Process.GetProcesses())
			// {
			// 	_processes.Add(new ProcessModel(process));
			// }
			List<ProcessModel> currentList = new List<ProcessModel>(_processes);
			List<ProcessModel> found = new List<ProcessModel>();
			List<ProcessModel> newList = new List<ProcessModel>();
			_processes.Clear();
			await Task.Run(() =>
			{
				Process.GetProcesses().AsParallel().ForAll(process =>
				{
					var idx = currentList.FindIndex(p => p.Id == process.Id);
					if (idx >= 0)
					{
						var p = currentList[idx];
						found.Add(p);
						p.NextTick();
					}
					else
					{
						var p = new ProcessModel(process);
						newList.Add(p);
					}
				});
			});
			found.ForEach(_processes.Add);
			newList.ForEach(_processes.Add);
			found.ForEach(f => currentList.Remove(f));
			currentList.ForEach(p => p.Dispose());
		}

		private void KillProcess()
		{
			try
			{
				SelectedProcess!.Kill();
				LoadProcesses();
			}
			catch (Exception ex)
			{
				_errorMessageService.ShowError($"Nie udało się zabić procesu: {ex.Message}");
			}
		}

		private void ChangePriority()
		{
			try
			{
				SelectedProcess!.Priority = SelectedPriority;
			}
			catch (Exception ex)
			{
				_errorMessageService.ShowError($"Nie udało się zmienić priorytetu: {ex.Message}");
			}
		}
	}
}
