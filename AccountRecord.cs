using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwd
{
    public class AccountRecord(int id, string title, string description, string login, string password, List<string> emails, List<string> numbers)
	{
		public int Id { get; } = id;

		public string Title { get; set; } = title;
		public string Description { get; set; } = description;

		public string Login { get; set; } = login;
		public string Password { get; set; } = password;

		public List<string> EmailList { get; set; } = emails;
		public List<string> NumberList { get; set; } = numbers;
    }
}
