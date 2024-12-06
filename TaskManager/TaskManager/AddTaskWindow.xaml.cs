using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.ViewModels;

namespace TaskManager
{
	/// <summary>
	/// Interaction logic for AddTaskWindow.xaml
	/// </summary>
	public partial class AddTaskWindow : Window
	{
		public AddTaskWindow(TaskModel task)
		{
			InitializeComponent();
			DataContext = new AddTaskViewModel(task, this, new MessageBoxErrorService());
		}
	}
}
