using Google.Protobuf.Compiler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwd
{
    public class AccountRecordViewModel : INotifyPropertyChanged
    {
		public ObservableCollection<AccountRecord> RecordList { get; set; }

		public AccountRecordViewModel()
		{
			RecordList = new ObservableCollection<AccountRecord>();
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
