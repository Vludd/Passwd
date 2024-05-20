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
    public partial class RecordEditor : Window
    {
        public RecordEditor()
        {
            InitializeComponent();
			Owner = Application.Current.MainWindow;
		}

		private void AcceptDialogue_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(TitleText.Text.Trim()) ||
				string.IsNullOrEmpty(LoginText.Text.Trim()) ||
				string.IsNullOrEmpty(PassMask.Text.Trim()))
			{
				MessageWindow message = new("Required fields is not filled!", "Not all required fields are filled in!", MessageWindowType.OK, this);
				message.ShowDialog();
				return;
			}

            DialogResult = true;
		}
	}
}
