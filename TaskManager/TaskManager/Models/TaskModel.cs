using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DateTime = System.DateTime;

namespace TaskManager.Models
{
	public class TaskModel : INotifyPropertyChanged
	{
		public string Name { get; set; }
		private DateTime _scheduledTime;
		private int _executionCount;

		public DateTime ScheduledTime
		{
			get => _scheduledTime;
			set
			{
				_scheduledTime = value;
				OnPropertyChanged();
			}
		}

		public string Command { get; set; }
		public bool IsCyclic { get; set; }

		public int ExecutionCount
		{
			get => _executionCount;
			set
			{
				_executionCount = value;
				OnPropertyChanged();
			}
		}

		public TimeSpan? Interval { get; set; }
		public bool IsDone { get; set; }

		public string Tooltip
		{
			get
			{
				Task.Run(() =>
				{
					Task.Delay(TimeSpan.FromSeconds(1));
					OnPropertyChanged();
				});
				if (IsCyclic)
				{
					return $@"Czas do następnego uruchomienia: {(ScheduledTime - DateTime.Now):hh\:mm\:ss}";
				}
				return ExecutionCount > 0
					? "Zadanie już zostało wykonane"
					: $@"Czas do uruchomienia: {(ScheduledTime - DateTime.Now):hh\:mm\:ss}";
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
