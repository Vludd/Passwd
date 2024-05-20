using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Passwd.Resources;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data.SQLite;

namespace Passwd
{
    public class DBQuery
    {
		private static void DropTable()
		{
			SQLiteConnection sqlConnection = DBHelper.Connect();

			if (sqlConnection.State != ConnectionState.Open) sqlConnection.Open();

			try
			{
				SQLiteCommand sqlCommand = new();

				string sqlQuery = $"DROP TABLE {DBHelper.TableName};";

				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = sqlQuery;

				var exec = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				sqlConnection.Close();
				sqlConnection.Dispose();
			}
		}

		private static void CreateTable()
		{
			SQLiteConnection sqlConnection = DBHelper.Connect();

			if (sqlConnection.State != ConnectionState.Open) sqlConnection.Open();

			try
			{
				SQLiteCommand sqlCommand = new();

				string sqlQuery = $"CREATE TABLE IF NOT EXISTS {DBHelper.TableName} (" +
					"id INTEGER NOT NULL UNIQUE PRIMARY KEY AUTOINCREMENT," +
					"title VARCHAR(50) NOT NULL," +
					"description VARCHAR(100)," +
					"login VARCHAR(50) NOT NULL," +
					"pass VARCHAR(100) NOT NULL," +
					"email_list VARCHAR(200)," +
					"number_list VARCHAR(200)" +
					");";

				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = sqlQuery;

				var exec = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				sqlConnection.Close();
				sqlConnection.Dispose();
			}
		}

		public static void AddNewRecord(string title, string description, string login, 
			string password, string email_list, string number_list)
		{
			SQLiteConnection sqlConnection = DBHelper.Connect();

			if (sqlConnection.State != ConnectionState.Open) sqlConnection.Open();

			try
			{
				SQLiteCommand sqlCommand = new();

				string sqlQuery = $"INSERT INTO {DBHelper.TableName} " +
					"(title, description, login, pass, email_list, number_list)\n" +
					"VALUES (@title, @description, @login, @pass, @email_list, @number_list);";

				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = sqlQuery;

				sqlCommand.Parameters.Add("@title", DbType.String).Value = title;
				sqlCommand.Parameters.Add("@description", DbType.String).Value = description;
				sqlCommand.Parameters.Add("@login", DbType.String).Value = login;
				sqlCommand.Parameters.Add("@pass", DbType.String).Value = password;
				sqlCommand.Parameters.Add("@email_list", DbType.String).Value = email_list;
				sqlCommand.Parameters.Add("@number_list", DbType.String).Value = number_list;

				var exec = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				sqlConnection.Close();
				sqlConnection.Dispose();
			}
		}

		public static void GetAllRecords(ObservableCollection<AccountRecord> recordList)
		{
			CreateTable();

			SQLiteConnection sqlConnection = DBHelper.Connect();

			if (sqlConnection.State != ConnectionState.Open) sqlConnection.Open();

			try
			{
				SQLiteCommand sqlCommand = new();

				string sqlQuery = $"SELECT * FROM {DBHelper.TableName};";

				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = sqlQuery;

				using DbDataReader reader = sqlCommand.ExecuteReader();

				recordList.Clear();

				while (reader.Read())
				{
					List<string> emailList = new List<string>();
					string emailListText = reader.GetString("email_list");

					string[] emails = emailListText.Split([',']);

					foreach (string email in emails)
					{
						emailList.Add(email);
					}

					List<string> numberList = new List<string>();
					string numberListText = reader.GetString("number_list");

					string[] numbers = numberListText.Split([',']);

					foreach (string number in numbers)
					{
						numberList.Add(number);
					}

					recordList.Add(new AccountRecord(
						reader.GetInt32("id"),
						reader.GetString("title"),
						reader.GetString("description"),
						reader.GetString("login"),
						reader.GetString("pass"),
						emailList,
						numberList
					));
				}

				reader.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				sqlConnection.Close();
				sqlConnection.Dispose();
			}
		}
		
		public static void DeleteRecord(int recordId)
		{
			SQLiteConnection sqlConnection = DBHelper.Connect();

			if (sqlConnection.State != ConnectionState.Open) sqlConnection.Open();

			try
			{
				SQLiteCommand sqlCommand = new();

				string sqlQuery = $"DELETE FROM {DBHelper.TableName} " +
					$"WHERE id = {recordId};";

				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = sqlQuery;

				int rowCount = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				sqlConnection.Close();
				sqlConnection.Dispose();
			}
		}

		public static void UpdateRecord(int recordId, string title, string description, string login,
			string password, string email_list, string number_list)
		{
			SQLiteConnection sqlConnection = DBHelper.Connect();

			if (sqlConnection.State != ConnectionState.Open) sqlConnection.Open();

			try
			{
				SQLiteCommand sqlCommand = new();

				string sqlQuery = $"UPDATE {DBHelper.TableName}\n" +
					$"SET title = @title, description = @description, login = @login, pass = @pass, email_list = @email_list, number_list = @number_list\n" +
					$"WHERE id = {recordId};";

				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = sqlQuery;

				sqlCommand.Parameters.Add("@title", DbType.String).Value = title;
				sqlCommand.Parameters.Add("@description", DbType.String).Value = description;
				sqlCommand.Parameters.Add("@login", DbType.String).Value = login;
				sqlCommand.Parameters.Add("@pass", DbType.String).Value = password;
				sqlCommand.Parameters.Add("@email_list", DbType.String).Value = email_list;
				sqlCommand.Parameters.Add("@number_list", DbType.String).Value = number_list;

				var exec = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				sqlConnection.Close();
				sqlConnection.Dispose();
			}
		}
	}
}
