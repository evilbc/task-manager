using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.ViewModels
{
	public class TaskViewModel : ITabViewModel
	{
		private readonly IErrorMessageService _errorMessageService;
		public string Header => "Zadania";

		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<TaskModel> Tasks { get; } = new ObservableCollection<TaskModel>();
		public TaskModel SelectedTask { get; set; }

		public ICommand AddTaskCommand { get; }
		private RelayCommand _removeTaskCommand;
		public ICommand RemoveTaskCommand => _removeTaskCommand;

		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		public TaskViewModel(IErrorMessageService errorMessageService)
		{
			_errorMessageService = errorMessageService;
			AddTaskCommand = new RelayCommand(AddTask);
			_removeTaskCommand = new RelayCommand(RemoveTask);

			Task.Run(() => RunTaskSchedulerAsync(_cancellationTokenSource.Token));
		}

		private void AddTask()
		{
			var newTask = new TaskModel
			{
				Name = "Nowe zadanie",
				ScheduledTime = DateTime.Now.AddMinutes(1),
				Command = "Notepad.exe",
				IsCyclic = false,
				ExecutionCount = 0
			};
			var addTaskWindow = new AddTaskWindow(newTask);
			if (addTaskWindow.ShowDialog() == true)
			{
				Tasks.Add(newTask);
			}
		}

		private void RemoveTask()
		{
			Tasks.Remove(SelectedTask);
		}

		private async Task RunTaskSchedulerAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var now = DateTime.Now;

				foreach (var task in Tasks.ToList())
				{
					if (task.ScheduledTime > now || task.IsDone) continue;
					ExecuteTask(task);

					if (task.IsCyclic && task.Interval.HasValue)
					{
						task.ScheduledTime = now.Add(task.Interval.Value);
					}
					else
					{
						task.IsDone = true;
					}
				}

				await Task.Delay(1000, cancellationToken);
			}
		}

		private void ExecuteTask(TaskModel task)
		{
			task.ExecutionCount++;

			try
			{
				var process = System.Diagnostics.Process.Start(task.Command);
				process?.Dispose();
			}
			catch (Exception ex)
			{
				_errorMessageService.ShowError($"Nie udało się uruchomić zadania {task.Name}: {ex.Message}");
			}
		}
	}
}
