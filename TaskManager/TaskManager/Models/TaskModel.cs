using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
	public class TaskModel
	{
		public string Name { get; set; }
		public DateTime ScheduledTime { get; set; }
		public string Command { get; set; }
		public bool IsCyclic { get; set; }
		public int ExecutionCount { get; set; }
		public TimeSpan? Interval { get; set; }

		public string Tooltip => IsCyclic
			? $"Czas do następnego uruchomienia: {(ScheduledTime - DateTime.Now):hh\\:mm\\:ss}"
			: $"Czas do uruchomienia: {(ScheduledTime - DateTime.Now):hh\\:mm\\:ss}";
	}
}
