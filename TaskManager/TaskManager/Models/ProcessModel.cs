using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
	public class ProcessModel
	{
		private Process Process { get; }
		public int Id => Process.Id;
		public string Name => Process.ProcessName;
		public List<string> ThreadInfo { get; private set; }
		public List<string> ModuleInfo { get; private set; }
		public string MemoryUsage { get; private set; }
		public string StartTime { get; private set; }

		public ProcessPriorityClass? Priority
		{
			get
			{
				try
				{
					return Process.PriorityClass;
				}
				catch (Exception)
				{
					return null;
				}
			}
			set => Process.PriorityClass = value!.Value;
		}

		public ProcessModel(Process process)
		{
			Process = process;
		}

		public void UpdateDetails()
		{
			UpdateThreadInfo();
			UpdateModuleInfo();
			MemoryUsage = Process.WorkingSet64.ToStringAsMemory();
			StartTime = Process.StartTime.ToString(CultureInfo.InvariantCulture);

		}

		public void Kill()
		{
			Process.Kill();
		}

		private void UpdateThreadInfo()
		{
			ThreadInfo = new List<string>();
			try
			{
				foreach (ProcessThread thread in Process.Threads)
				{
					ThreadInfo.Add($"Wątek ID: {thread.Id}, stan: {thread.ThreadState}");
				}
			}
			catch (Exception e)
			{
				ThreadInfo.Add($"Brak dostępu do wątków: {e.Message}");
			}
		}

		private void UpdateModuleInfo()
		{
			ModuleInfo = new List<string>();
			try
			{
				foreach (ProcessModule module in Process.Modules)
				{
					ModuleInfo.Add($"{module.ModuleName} ({module.FileName})");
				}
			}
			catch (Exception e)
			{
				ModuleInfo.Add($"Brak dostępu do modułów: {e.Message}");
			}
		}

	}
}

