using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
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

		public response register(register a)
		{
			response res = new response();
			try
			{
				if (a.Password!=a.ConfirmPassword)
				{
					res.isSuccess = false;
					res.message = "Password and ConfirmPassword donot match";
					return res;
				}

				var conn = new SqlConnection(configuration.GetConnectionString("default"));
				conn.Open();
				conn.Execute("INSERT INTO register (Name, Password, ConfirmPassword, Email) values (@UserName, @Password, @ConfirmPassword, @Email)", a);

				res.isSuccess = true;
				res.message = "successfully registered";
				

			}
			catch (Exception ex)
			{
				res.isSuccess = false;
				res.message = ex.Message;
			}
			return res;
		}

		public response login(login a)
		{
			response res = new response();
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
					var tok = tokenHandler.WriteToken(token);
					//return (display);
					res.isSuccess = true;
					res.message = "Successfully Loggedin";
					res.token = tok;
					return res;
				}
				
			}
			res.isSuccess = false;
			res.message = "User Not Found";
			return res;
		}

		public caloriecount dietplan(calculate cal)
		{
			if (string.IsNullOrEmpty(cal.gender) || string.IsNullOrEmpty(cal.lifestyle))
			{
				//return new responsecal { status = "unsuccessful" };
			}
			cal.lifestyle.ToLower();
			cal.gender = cal.gender.ToLower();
			double bmr = 0;
			double maintenance = 0;
			caloriecount ddal = new caloriecount();

			if (cal.gender == "male")
			{
				if (cal.lifestyle == "s")
				{
					if (cal.goal == "lose fat")
					{
						if (cal.experience == "b")
						{
							if (cal.gender == "male")
							{
								if (cal.bodyfat >= 18)
								{
									bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
									maintenance = bmr * 1.3;
									ddal.calories = maintenance-(0.2) * maintenance;
									var calfat = (0.25) * ddal.calories;
									ddal.fat = calfat;
									ddal.protein = (1.5) * cal.weight;
									var calprotein = ddal.protein * 4;
									var calfromproteinandfat = calfat + calprotein;
									var calcarb = ddal.calories - (calfromproteinandfat);
									ddal.carbohydrate = calcarb / 4;
									ddal.maintenance = maintenance;

								}
							}
						}
					}
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
			//caloriecount res = dietgen(maintenance, cal.bodyfat, cal.gender, cal.experience, cal.goal,cal.weight);
			return ddal;
			//return new responsecal { bmr = bmr, maintenance = maintenance, status = "success" };
		//	return $"BMR = {bmr} and Maintenance Calories = {maintenance}";
		}


		//public caloriecount dietgen(double maintenance, double fat, string gender, string experience, string goal, double weight)
		//{
		//	caloriecount cal = new caloriecount();
		//	if (goal == "lose fat")
		//	{
		//		if (experience == "b")
		//		{
		//			if (gender == "male")
		//			{
		//				if (fat >= 18)
		//				{
		//					cal.calories = maintenance - ((20 / 100) * maintenance);
		//					var calfat = (25 / 100) * cal.calories;
		//					cal.fat = calfat;
		//					cal.protein = (1.5) * weight;
		//					var calprotein = cal.protein * 4;
		//					var calfromproteinandfat = calfat + calprotein;
		//					var calcarb = cal.calories - (calfromproteinandfat);
		//					cal.carbohydrate = calcarb / 4;
		//					cal.maintenance = maintenance;
							
		//				}
		//			}
		//		}
		//	}

		//	if (goal == "lose fat")
		//	{
		//		if (experience == "b")
		//		{
		//			if (gender == "female")
		//			{
		//				if (fat >= 28)
		//				{
		//					cal.calories = maintenance - ((20 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}
		//	if (goal == "build muscle")
		//	{
		//		if (experience == "b")
		//		{
		//			if (gender == "male")
		//			{
		//				if (fat <= 12)
		//				{
		//					cal.calories = maintenance + ((25 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}
			
		//	if (goal == "build muscle")
		//	{
		//		if (experience == "b")
		//		{
		//			if (gender == "female")
		//			{
		//				if (fat <= 22)
		//				{
		//					cal.calories = maintenance + ((25 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}

		//	if (goal == "build muscle lose fat")
		//	{
		//		if (experience == "b")
		//		{
		//			if (gender == "male")
		//			{
		//				if (fat > 12 && fat<18)
		//				{
		//					cal.calories = maintenance;
		//				}
		//			}
		//		}
		//	}
		//	if (goal == "build muscle lose fat")
		//	{
		//		if (experience == "b")
		//		{
		//			if (gender == "female")
		//			{
		//				if (fat > 22 && fat < 28)
		//				{
		//					cal.calories = maintenance;
		//				}
		//			}
		//		}
		//	}
		//	//intermediate
		//	if (goal == "lose fat")
		//	{
		//		if (experience == "i")
		//		{
		//			if (gender == "male")
		//			{
		//				if (fat >= 18)
		//				{
		//					cal.calories = maintenance - ((20 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}

		//	if (goal == "lose fat")
		//	{
		//		if (experience == "i")
		//		{
		//			if (gender == "female")
		//			{
		//				if (fat >= 28)
		//				{
		//					cal.calories = maintenance - ((20 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}
		//	if (goal == "build muscle")
		//	{
		//		if (experience == "i")
		//		{
		//			if (gender == "male")
		//			{
		//				if (fat <= 12)
		//				{
		//					cal.calories = maintenance + ((15 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}

		//	if (goal == "build muscle")
		//	{
		//		if (experience == "i")
		//		{
		//			if (gender == "female")
		//			{
		//				if (fat <= 22)
		//				{
		//					cal.calories = maintenance + ((15 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}

		//	if (goal == "build muscle lose fat")
		//	{
		//		if (experience == "i")
		//		{
		//			if (gender == "male")
		//			{
		//				if (fat > 12 && fat < 18)
		//				{
		//					cal.calories = maintenance;
		//				}
		//			}
		//		}
		//	}

		//	if (goal == "build muscle lose fat")
		//	{
		//		if (experience == "i")
		//		{
		//			if (gender == "female")
		//			{
		//				if (fat > 22 && fat < 28)
		//				{
		//					cal.calories = maintenance;
		//				}
		//			}
		//		}
		//	}
		//	//advanced
		//	if (goal == "lose fat")
		//	{
		//		if (experience == "b")
		//		{
		//			if (gender == "male")
		//			{
		//				if (fat >= 18)
		//				{
		//					cal.calories = maintenance - ((15 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}

		//	if (goal == "lose fat")
		//	{
		//		if (experience == "a")
		//		{
		//			if (gender == "female")
		//			{
		//				if (fat >= 28)
		//				{
		//					cal.calories = maintenance - ((15 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}
		//	if (goal == "build muscle")
		//	{
		//		if (experience == "a")
		//		{
		//			if (gender == "male")
		//			{
		//				if (fat <= 12)
		//				{
		//					cal.calories = maintenance + ((10 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}

		//	if (goal == "build muscle")
		//	{
		//		if (experience == "a")
		//		{
		//			if (gender == "female")
		//			{
		//				if (fat <= 22)
		//				{
		//					cal.calories = maintenance + ((10 / 100) * maintenance);
		//				}
		//			}
		//		}
		//	}

		//	return cal;
		//}

		public responsebmi bmi(double height, double weight)
		{
			string path;
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
			if (data == "Obese")
			{
				 path = @"C:\Users\basne\source\repos\Kasrat\Kasrat\content\OBESE WORKOUT.pdf";
			}
			else
			{
				 path = @"C:\Users\basne\source\repos\Kasrat\Kasrat\content\UNDERWEIGHT WORKOUT.pdf";
			}
			return new responsebmi { bmi = index, result = data, fileurl = path};
		}
	}
}
