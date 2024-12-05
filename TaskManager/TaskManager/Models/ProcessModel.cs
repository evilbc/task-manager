using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
	public class ProcessModel : IDisposable
	{
		private Process Process { get; }
		private PerformanceCounter CpuCounter { get; }
		private PerformanceCounter DiskCounter { get; }
		private PerformanceCounter MemoryCounter { get; }
		private float _cpuUsage { get; set; }
		private float _diskUsage { get; set; }
		private float _memoryUsage { get; set; }
		public int Id => Process.Id;
		public string Name => Process.ProcessName;
		public string MemoryUsage => $"{_memoryUsage} MB";
		public string CpuUsage => $"{_cpuUsage} %";
		public string DiskUsage => $"{_diskUsage} MB/s";

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
			CpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
			DiskCounter = new PerformanceCounter("Process", "IO Data Bytes/sec", process.ProcessName, true);
			MemoryCounter = new PerformanceCounter("Process", "Working Set", process.ProcessName, true);
			NextTick();
		}

		public void NextTick()
		{
			_cpuUsage = CpuCounter.NextValue() / Environment.ProcessorCount;
			_diskUsage = DiskCounter.NextValue() / 1024 / 1024;
			_memoryUsage = MemoryCounter.NextValue();
		}

		public void Kill()
		{
			Process.Kill();
		}

		public void Dispose()
		{
			DiskCounter.Dispose();
			CpuCounter.Dispose();
			MemoryCounter.Dispose();
		}
	}
}

