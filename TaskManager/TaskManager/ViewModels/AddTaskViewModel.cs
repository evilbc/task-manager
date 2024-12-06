using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
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
	public class AddTaskViewModel
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private readonly TaskModel _task;
		private readonly Window _window;
		private readonly IErrorMessageService _errorMessageService;

		public string Name
		{
			get => _task.Name;
			set
			{
				_task.Name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public DateTime? ScheduledDate { get; set; }
		public string ScheduledTime { get; set; }
		public string Command
		{
			get => _task.Command;
			set
			{
				_task.Command = value;
				OnPropertyChanged(nameof(Command));
			}
		}

		public bool IsCyclic
		{
			get => _task.IsCyclic;
			set
			{
				_task.IsCyclic = value;
				OnPropertyChanged(nameof(IsCyclic));
			}
		}

		public string Interval
		{
			get => _task.Interval?.ToString(@"hh\:mm\:ss");
			set
			{
				if (TimeSpan.TryParse(value, out var interval))
				{
					_task.Interval = interval;
				}
				else
				{
					_task.Interval = null;
				}
				OnPropertyChanged(nameof(Interval));
			}
		}


		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public AddTaskViewModel(TaskModel task, Window window, IErrorMessageService errorMessageService)
		{
			_task = task;
			_window = window;
			_errorMessageService = errorMessageService;

			ScheduledDate = task.ScheduledTime.Date;
			ScheduledTime = task.ScheduledTime.ToShortTimeString();

			SaveCommand = new RelayCommand(Save);
			CancelCommand = new RelayCommand(Cancel);
		}

		private void Save()
		{
			if (ScheduledDate.HasValue && TimeSpan.TryParse(ScheduledTime, out var time))
			{
				_task.ScheduledTime = ScheduledDate.Value.Add(time);

				if (_task.IsCyclic && !_task.Interval.HasValue)
				{
					_errorMessageService.ShowError("Wprowadź poprawny interwał dla zadania cyklicznego.");
					return;
				}

				_window.DialogResult = true;
			}
			else
			{
				_errorMessageService.ShowError("Wprowadź poprawną datę i godzinę.");
			}
		}

		private void Cancel()
		{
			_window.Close();
		}
	}
}
