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

namespace Passwd
{
	public partial class MessageWindow : Window
	{
		public MessageWindow(string messageTitle, string messageText, MessageWindowType messageType = MessageWindowType.OK, Window? owner = null)
		{
			InitializeComponent();

			Title = messageTitle;
			MessageBlock.Text = messageText;

			switch (messageType)
			{
				case MessageWindowType.OK:
					OKGrid.Visibility = Visibility.Visible;
					OKCancelGrid.Visibility = Visibility.Hidden;
					YESNOGrid.Visibility = Visibility.Hidden;
					break;

				case MessageWindowType.OK_CANCEL:
					OKGrid.Visibility = Visibility.Hidden;
					OKCancelGrid.Visibility = Visibility.Visible;
					YESNOGrid.Visibility = Visibility.Hidden;
					break;

				case MessageWindowType.YES_NO:
					OKGrid.Visibility = Visibility.Hidden;
					OKCancelGrid.Visibility = Visibility.Hidden;
					YESNOGrid.Visibility = Visibility.Visible;
					break;
			}

			if (owner != null)
			{
				Owner = owner;
			}
			else
			{
				Owner = Application.Current.MainWindow;
			}
		}

		public static void Show(string messageTitle, string messageText, MessageWindowType messageType = MessageWindowType.OK)
		{
			MessageWindow messageWindow = new MessageWindow(messageTitle, messageText, messageType);
			messageWindow.ShowDialog();
		}

		public static void Show(Window owner, string messageTitle, string messageText, MessageWindowType messageType = MessageWindowType.OK)
		{
			MessageWindow messageWindow = new MessageWindow(messageTitle, messageText, messageType, owner);
			messageWindow.ShowDialog();
		}

		private void AcceptDialogue_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void YesDialogue_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void OKDialogue_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}

	public enum MessageWindowType
	{
		OK,
		OK_CANCEL,
		YES_NO
	}
}
