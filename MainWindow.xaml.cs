using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Passwd
{
    public partial class MainWindow : Window
    {
		[GeneratedRegex("[0-9]+")] private static partial Regex IgnoreNonNumber();
		[GeneratedRegex(@"[^\d]")] private static partial Regex IgnorePaste();

		public static AccountRecordViewModel ViewModel { get; set; } = new AccountRecordViewModel();

		private bool pinAccess = false;

		private int pinAttempts = 5;

		private int prevSelectedRecord = -1;

		private float timeElapsed;

		private float startTime;

		private System.Timers.Timer notificationTimer = new System.Timers.Timer();

        public MainWindow()
        {
            InitializeComponent();

			DataContext = ViewModel;

			DBQuery.GetAllRecords(ViewModel.RecordList);
			ContentGrid.IsEnabled = false;

			if (ViewModel.RecordList.Count > 0)
			{
				NotDBOverlay.Visibility = Visibility.Hidden;
			}
			else
			{
				NotDBOverlay.Visibility = Visibility.Visible;
			}

			if (pinAccess)
			{
				MainGrid.Visibility = Visibility.Hidden;
				LoginGrid.Visibility = Visibility.Visible;
				InputPinCode.Focus();
			}
		}

        private void InputPinCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = IgnoreNonNumber();
            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void InputPinCode_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Regex myRegex = IgnorePaste();
            if (myRegex.IsMatch(InputPinCode.Password))
                InputPinCode.Password = myRegex.Replace(InputPinCode.Password, "");
        }

		private void PasswordValidating()
		{
			if (InputPinCode.Password.Length > 0)
			{
				if (InputPinCode.Password == "0000")
				{
					LoginGrid.Visibility = Visibility.Hidden;
					MainGrid.Visibility = Visibility.Visible;
				}
				else
				{
					if (pinAttempts > 0)
					{
						PinValidationMsg.Text = $"Invalid PIN! Attempts left: {pinAttempts--}";
						PinValidationMsg.Visibility = Visibility.Visible;
					}
					else
					{
						MessageWindow message = new("Access denied!", "Attempts have been exhausted.\n" +
							"Repeated incorrect login attempts will permanently block access!\n\n" +
							"Application terminate...");
						message.ShowDialog();
						Application.Current.Shutdown();
					}
				}
			}
			else
			{
				if (pinAttempts > 0)
				{
					PinValidationMsg.Text = $"Invalid PIN! Attempts left: {pinAttempts--}";
					PinValidationMsg.Visibility = Visibility.Visible;
				}
				else
				{
					Application.Current.Shutdown();
				}
			}
		}

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
			PasswordValidating();
		}

        private void LogInButton_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
				PasswordValidating();
			}
        }

		private void CopyLogin_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(LoginText.Text)) Clipboard.SetText(LoginText.Text);
		}

		private void CopyEmail_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(EmailText.Text)) Clipboard.SetText(EmailText.Text);
		}

		private void CopyPassword_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(PassTextMask.Password)) Clipboard.SetText(PassTextMask.Password); 
		}

		private void ShowPassButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			PassTextMask.Visibility = Visibility.Hidden;
			PassTextUnmask.Visibility = Visibility.Visible;
		}

		private void ShowPassButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			PassTextMask.Visibility = Visibility.Visible;
			PassTextUnmask.Visibility = Visibility.Hidden;
		}

		private void PassTextMask_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = true;
		}

		private void CopyNumberList_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(NumberText.Text)) Clipboard.SetText(NumberText.Text);
		}

		private void RecordsListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var recordIndex = RecordListView.SelectedIndex;

			if (recordIndex == -1)
			{
				ResetFieldsAfterAction();
				prevSelectedRecord = -1;
				return;
			}

			if (recordIndex == prevSelectedRecord)
			{
				return;
			}

			prevSelectedRecord = recordIndex;

			ContentGrid.IsEnabled = true;

			LoginText.Text = ViewModel.RecordList[recordIndex].Login;
			PassTextMask.Password = ViewModel.RecordList[recordIndex].Password;
			PassTextUnmask.Text = ViewModel.RecordList[recordIndex].Password;

			var emailList = ViewModel.RecordList[recordIndex].EmailList;
			foreach (var email in emailList)
			{
				if (emailList.IndexOf(email) == 0)
				{
					EmailText.Text = email;
				}
				else
				{
					EmailText.Text += $", {email}";
				}
			}

			var numberList = ViewModel.RecordList[recordIndex].NumberList;
			foreach (var number in numberList)
			{
				if (numberList.IndexOf(number) == 0)
				{
					NumberText.Text = number;
				}
				else
				{
					NumberText.Text += $", {number}";
				}
			}

			RecordInfo.Text = ViewModel.RecordList[recordIndex].Description;
		}

		private void AddNewRecord_Click(object sender, RoutedEventArgs e)
		{
			var addRecordWindow = new RecordEditor();
			if (addRecordWindow.ShowDialog().Equals(true))
			{
				string newRecordTitle = addRecordWindow.TitleText.Text;
				string newRecordDesc = addRecordWindow.DescriptionText.Text;
				string newRecordLogin = addRecordWindow.LoginText.Text;
				string newRecordPass = addRecordWindow.PassMask.Text;

				string newRecordEmails = addRecordWindow.EmailText.Text.Replace(" ", "");
				string newRecordNumbers = addRecordWindow.NumberText.Text.Replace(" ", "");

				DBQuery.AddNewRecord(newRecordTitle, newRecordDesc, newRecordLogin, newRecordPass, newRecordEmails, newRecordNumbers);
				DBQuery.GetAllRecords(ViewModel.RecordList);

				ResetFieldsAfterAction();

				if (ViewModel.RecordList.Count > 0)
				{
					NotDBOverlay.Visibility = Visibility.Hidden;
				}
				else
				{
					NotDBOverlay.Visibility = Visibility.Visible;
				}
			}
		}

		private void DeleteRecord_Click(object sender, RoutedEventArgs e)
		{
			var recordIndex = RecordListView.SelectedIndex;

			if (recordIndex == -1)
			{
				MessageWindow messageWindow = new("Record not selected!", "No record has been selected!");
				messageWindow.ShowDialog();
				return;
			}

			MessageWindow message = new("Confirm delete", "Do you really want to delete this record?", MessageWindowType.YES_NO);

			if (message.ShowDialog().Equals(true))
			{
				DBQuery.DeleteRecord(ViewModel.RecordList[recordIndex].Id);
				DBQuery.GetAllRecords(ViewModel.RecordList);

				ResetFieldsAfterAction();

				if (ViewModel.RecordList.Count > 0)
				{
					NotDBOverlay.Visibility = Visibility.Hidden;
				}
				else
				{
					NotDBOverlay.Visibility = Visibility.Visible;
				}
			}
		}

		private void EditRecord_Click(object sender, RoutedEventArgs e)
		{
			var editRecordWindow = new RecordEditor();

			var recordIndex = RecordListView.SelectedIndex;

			if (recordIndex == -1)
			{
				MessageWindow messageWindow = new("Record not selected!", "No record has been selected!");
				messageWindow.ShowDialog();
				return;
			}

			editRecordWindow.TitleText.Text = ViewModel.RecordList[recordIndex].Title;
			editRecordWindow.DescriptionText.Text = ViewModel.RecordList[recordIndex].Description;

			editRecordWindow.LoginText.Text = ViewModel.RecordList[recordIndex].Login;
			editRecordWindow.PassMask.Text = ViewModel.RecordList[recordIndex].Password;

			var emailList = ViewModel.RecordList[recordIndex].EmailList;
			foreach (var email in emailList)
			{
				if (emailList.IndexOf(email) == 0)
				{
					editRecordWindow.EmailText.Text = email;
				}
				else
				{
					editRecordWindow.EmailText.Text += $", {email}";
				}
			}

			var numberList = ViewModel.RecordList[recordIndex].NumberList;
			foreach (var number in numberList)
			{
				if (numberList.IndexOf(number) == 0)
				{
					editRecordWindow.NumberText.Text = number;
				}
				else
				{
					editRecordWindow.NumberText.Text += $", {number}";
				}
			}

			if (editRecordWindow.ShowDialog().Equals(true))
			{
				string newRecordTitle = editRecordWindow.TitleText.Text;
				string newRecordDesc = editRecordWindow.DescriptionText.Text;
				string newRecordLogin = editRecordWindow.LoginText.Text;
				string newRecordPass = editRecordWindow.PassMask.Text;

				string newRecordEmails = editRecordWindow.EmailText.Text.Replace(" ", "");
				string newRecordNumbers = editRecordWindow.NumberText.Text.Replace(" ", "");

				DBQuery.UpdateRecord(ViewModel.RecordList[recordIndex].Id, newRecordTitle, newRecordDesc, newRecordLogin, newRecordPass, newRecordEmails, newRecordNumbers);
				DBQuery.GetAllRecords(ViewModel.RecordList);

				ShowNotification("Record has been updated!", 3);

				ResetFieldsAfterAction();
			}
		}

		private void ResetFieldsAfterAction()
		{
			LoginText.Text = "";
			PassTextMask.Password = "";
			PassTextUnmask.Text = "";
			EmailText.Text = "";
			NumberText.Text = "";
			RecordInfo.Text = "";

			RecordListView.SelectedIndex = -1;
			ContentGrid.IsEnabled = false;
		}

		private void RecordListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			RecordListView.UnselectAll();

			if (prevSelectedRecord == -1)
			{
				ResetFieldsAfterAction();
			}
		}

		private void ShowNotification(string text, float time)
		{
			NotificationLabel.Text = text;

			NotificationOverlay.Opacity = 1;
			NotificationOverlay.Visibility = Visibility.Visible;

			TimerStart(time);
		}

		private void TimerStart(float time)
		{
			timeElapsed = time * 1000;
			startTime = time * 1000;

			notificationTimer = new(1);
			notificationTimer.AutoReset = false;
			notificationTimer.Elapsed += new ElapsedEventHandler(NotificationTimerTick);
			notificationTimer.Start();
		}

		private void NotificationTimerTick(Object source, ElapsedEventArgs e)
		{
			var newOpacity = timeElapsed / startTime;

			//NotificationOverlay.Opacity = newOpacity;

			if (NotificationOverlay.Opacity <= 0)
			{
				NotificationOverlay.Visibility = Visibility.Hidden;
				notificationTimer.Stop();
			}

			if (timeElapsed <= 0)
			{
				NotificationOverlay.Visibility = Visibility.Hidden;
				notificationTimer.Stop();
			}

			timeElapsed--;
		}
	}
}