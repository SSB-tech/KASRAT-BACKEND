using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;

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

		public string login(login a)
		{
			var conn = new SqlConnection(configuration.GetConnectionString("default"));
			conn.Open();
			var data = conn.QueryFirstOrDefault("SELECT * FROM register WHERE Name = @Name AND Password = @password", new { Name = a.username, Password = a.password });
			//var display = conn.Query("SELECT * FROM register");
			if (data != null)
			{
				{
					//token generation
					var key = configuration.GetValue<string>("jwtconfig:key");
					var keybytes = Encoding.ASCII.GetBytes(key);

					var tokenHandler = new JwtSecurityTokenHandler();

					var tokenDescriptor = new SecurityTokenDescriptor()
					{
						Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
						{
						new Claim(ClaimTypes.NameIdentifier, a.username)
						}),
						Expires = DateTime.UtcNow.AddMinutes(30),
						SigningCredentials = new SigningCredentials(
							 new SymmetricSecurityKey(keybytes), SecurityAlgorithms.HmacSha256Signature
							 )
					};
					var token = tokenHandler.CreateToken(tokenDescriptor);
					return tokenHandler.WriteToken(token);
					//return (display);
				}
				
			}
			return ("User Not Found");
		}

		public responsecal dietplan(calculate cal)
		{
			if (string.IsNullOrEmpty(cal.gender) || string.IsNullOrEmpty(cal.lifestyle))
			{
				return new responsecal { status = "unsuccessful" };
			}
			cal.lifestyle.ToLower();
			cal.gender = cal.gender.ToLower();
			double bmr = 0;
			double maintenance = 0;
			if (cal.gender == "male")
			{
				if (cal.lifestyle == "s")
				{
					bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
					maintenance = bmr * 1.3;
				}
				else if (cal.lifestyle == "la")
				{
					bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
					maintenance = bmr * 1.6;
				}
				else if (cal.lifestyle == "ma")
				{
					bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
					maintenance = bmr * 1.8;
				}
				else if (cal.lifestyle == "ha")
				{
					bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
					maintenance = bmr * 2;
				}
			}
			else if (cal.gender == "female")
			{
				if (cal.lifestyle == "s")
				{
					bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
					maintenance = bmr * 1.3;
				}
				else if (cal.lifestyle == "la")
				{
					bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
					maintenance = bmr * 1.6;
				}
				else if (cal.lifestyle == "ma")
				{
					bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
					maintenance = bmr * 1.8;
				}
				else if (cal.lifestyle == "ha")
				{
					bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
					maintenance = bmr * 2;
				}
			}
			return new responsecal { bmr = bmr, maintenance = maintenance, status = "success" };
		//	return $"BMR = {bmr} and Maintenance Calories = {maintenance}";
		}

		public responsebmi bmi(double height, double weight)
		{
			string data = "";
			var index = weight / (height * height);

			if (index < 18.5)
			{
				 data = "Underweight";
			}

			else if (index>18.5 && index < 24.9)
			{
				data = "Normal";
			}

			else if (index > 24.9 && index < 29.9)
			{
				data = "Overweight";
			}
			else if (index > 29.9 && index < 34.9)
			{
				data = "Obese";
			}
			else if (index > 34.9)
			{
				data = "Extremely Obese";
			}
			//pdf

			//byte[] fileBytes = System.IO.File.ReadAllBytes("C:\\Users\\basne\\source\\repos\\Kasrat\\Kasrat\\content\\cutting.pdf");
			//MemoryStream stream = new MemoryStream();

			//var stream = new FileStream(@"C:\Users\basne\source\repos\Kasrat\Kasrat\content\cutting.pdf", FileMode.Open);
			//stream.Position= 0;
			//return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = "ssb.pdf" };
			//FileStreamResult file = new FileStreamResult(stream, "application/pdf") { FileDownloadName = "ssb.pdf" };
			string path = @"C:\Users\basne\source\repos\Kasrat\Kasrat\content\cutting.pdf";
			return new responsebmi { bmi = index, result = data, fileurl = path};
		}
	}
}
