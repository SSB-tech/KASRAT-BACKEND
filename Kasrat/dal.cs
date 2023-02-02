using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Kasrat
{
	public class dal
	{

		IConfiguration configuration;

		public dal(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public string register(model a)
		{
			try
			{
				var conn = new SqlConnection(configuration.GetConnectionString("default"));
				conn.Open();
				conn.Execute("INSERT INTO register (Name, Password, Contact, Email) values (@Name, @Password, @Contact, @Email)", a);
				return ("successful");

			}
			catch (Exception ex)
			{
				return ("error");
			}
		}

		public object login(login a)
		{	
				var conn = new SqlConnection(configuration.GetConnectionString("default"));
				conn.Open();
				var data = conn.QueryFirstOrDefault("SELECT * FROM register WHERE Name = @Name AND Password = @password", new { Name = a.username, Password = a.password });
			var display = conn.Query("SELECT * FROM register");
			if(data!= null)
			{
				return (display);
			}
			return ("error");	
		}
	}
}
