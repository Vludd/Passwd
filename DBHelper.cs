using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Passwd.Resources
{
    class DBHelper
    {
		public const string DBName = "passwddata";
		public const string TableName = "records";

		public static SQLiteConnection Connect()
		{
			var connection = new SQLiteConnection($"Data Source={DBName}.db");

			return connection;
		}
	}
}
