using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Passwd
{
	public partial class PopupWindow : Window
	{
		private System.Timers.Timer notificationTimer = new();
		private float timeElapsed;
		private float startTime;

		public PopupWindow(string messageText, float time)
		{
			InitializeComponent();

			Owner = Application.Current.MainWindow;

			NotificationLabel.Text = messageText;
			NotificationOverlay.Opacity = 1;

			TimerStart(time);
		}

		private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		private void TimerStart(float time)
		{
			timeElapsed = time * 1000;
			startTime = time * 1000;

			notificationTimer = new(1);
			notificationTimer.Elapsed += new ElapsedEventHandler(NotificationTimerTick);
			notificationTimer.Start();
		}

		private void NotificationTimerTick(object source, ElapsedEventArgs e)
		{
			timeElapsed--;

			if (timeElapsed <= 0)
			{
				Close();
			}
		}
	}
}
