using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaskManager.Services
{
	public class MessageBoxErrorService : IErrorMessageService
	{
		public void ShowError(string message)
		{
			MessageBox.Show(message);
		}
	}
}
