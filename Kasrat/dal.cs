using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
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
						new Claim(ClaimTypes.NameIdentifier, data.ID.ToString()), // add user ID as a claim
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

        //public caloriecount dietplan(calculate cal)
        //{
        //	if (string.IsNullOrEmpty(cal.gender) || string.IsNullOrEmpty(cal.lifestyle))
        //	{
        //		//return new responsecal { status = "unsuccessful" };
        //	}
        //	cal.lifestyle.ToLower();
        //	cal.gender = cal.gender.ToLower();
        //	double bmr = 0;
        //	double maintenance = 0;
        //	caloriecount ddal = new caloriecount();

        //	if (cal.gender == "male")
        //	{
        //		if (cal.lifestyle == "s")
        //		{
        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance - (0.2) * maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //						ddal.calfromprotein = ddal.protein * 4;
        //						ddal.calfromcarb = ddal.carbohydrate * 4;
        //						ddal.calfromfat = ddal.fat * 9;

        //						ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //						//ddal.weight = cal.weight;
        //						//ddal.bodyfat = cal.bodyfat;
        //						var height = cal.height * 0.01;
        //						ddal.bmi = (cal.weight) / (height * height); 

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			else if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "b")
        //				{

        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance + ((0.25) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat > 12 && cal.bodyfat < 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance - ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance + ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat > 12 && cal.bodyfat < 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance - ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance + ((0.1) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //		}
        //		else if (cal.lifestyle == "la")
        //		{
        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance - (0.2) * maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			else if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "b")
        //				{

        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance + ((0.25) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";

        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat > 12 && cal.bodyfat < 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance - ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";

        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance + ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat > 12 && cal.bodyfat < 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance - ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance + ((0.1) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //		}
        //		else if (cal.lifestyle == "ma")
        //		{
        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance - (0.2) * maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			else if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "b")
        //				{

        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance + ((0.25) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat > 12 && cal.bodyfat < 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance - ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance + ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat > 12 && cal.bodyfat < 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance - ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance + ((0.1) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //		}

        //		else if (cal.lifestyle == "ha")
        //		{
        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance - (0.2) * maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			else if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "b")
        //				{

        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance + ((0.25) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat > 12 && cal.bodyfat < 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance - ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance + ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat > 12 && cal.bodyfat < 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat >= 18)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance - ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat <= 12)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance + ((0.1) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //		}
        //	}
        //	//-----------------------------------------------------------------------FEMALE-------------------------------------------------
        //	else if (cal.gender == "female")
        //	{
        //		if (cal.lifestyle == "s")
        //		{
        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance - ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}


        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance + ((0.25) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat > 22 && cal.bodyfat < 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //			//intermediate

        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance + ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance + ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat > 22 && cal.bodyfat < 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //			//advanced


        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance - ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}


        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.3;
        //						ddal.calories = maintenance + ((0.10) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //		}

        //		else if (cal.lifestyle == "la")
        //		{
        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance - ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                              return ddal;
        //					}
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance + ((0.25) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat > 22 && cal.bodyfat < 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //			//intermediate


        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance + ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance + ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}



        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat > 22 && cal.bodyfat < 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //			//advanced


        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance - ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}


        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.6;
        //						ddal.calories = maintenance + ((0.10) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/ 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //		}

        //		else if (cal.lifestyle == "ma")
        //		{
        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance - ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}


        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance + ((0.25) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat > 22 && cal.bodyfat < 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //			//intermediate


        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance + ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance + ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}



        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat > 22 && cal.bodyfat < 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //			//advanced


        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance - ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}


        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 1.8;
        //						ddal.calories = maintenance + ((0.10) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //		}

        //		else if (cal.lifestyle == "ha")
        //		{
        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance - ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}


        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance + ((0.25) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "b")
        //				{
        //					if (cal.bodyfat > 22 && cal.bodyfat < 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //			//intermediate


        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance + ((0.2) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}

        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance + ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}



        //			if (cal.goal == "build muscle lose fat")
        //			{
        //				if (cal.experience == "i")
        //				{
        //					if (cal.bodyfat > 22 && cal.bodyfat < 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance;
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //			//advanced


        //			if (cal.goal == "lose fat")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat >= 28)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance - ((0.15) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat/9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}


        //			if (cal.goal == "build muscle")
        //			{
        //				if (cal.experience == "a")
        //				{
        //					if (cal.bodyfat <= 22)
        //					{
        //						bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
        //						maintenance = bmr * 2;
        //						ddal.calories = maintenance + ((0.10) * maintenance);
        //						var calfat = (0.25) * ddal.calories;
        //						ddal.fat = calfat / 9;
        //						ddal.protein = (1.5) * cal.weight;
        //						var calprotein = ddal.protein * 4;
        //						var calfromproteinandfat = calfat + calprotein;
        //						var calcarb = ddal.calories - (calfromproteinandfat);
        //						ddal.carbohydrate = calcarb / 4;
        //						ddal.maintenance = maintenance;

        //                              ddal.calfromprotein = ddal.protein * 4;
        //                              ddal.calfromcarb = ddal.carbohydrate * 4;
        //                              ddal.calfromfat = ddal.fat * 9;

        //                              ddal.perfromprotein = ((ddal.calfromprotein) / (ddal.calories)) * 100;
        //                              ddal.perfromcarb = ((ddal.calfromcarb) / (ddal.calories)) * 100;
        //                              ddal.perfromfat = ((ddal.calfromfat) / (ddal.calories)) * 100;

        //                              ddal.message = "Your fat% is ideal for your goal";
        //                          }
        //                          ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
        //                          return ddal;
        //                      }
        //			}
        //		}
        //	}
        //	//caloriecount res = dietgen(maintenance, cal.bodyfat, cal.gender, cal.experience, cal.goal,cal.weight);
        //	return ddal;
        //	//return new responsecal { bmr = bmr, maintenance = maintenance, status = "success" };
        ////	return $"BMR = {bmr} and Maintenance Calories = {maintenance}";
        //}


        //----------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------

        public caloriecount malediet(double maintenance, double bmr, double weight, double height, double calories)
        {
            caloriecount ddal = new caloriecount();
            ddal.calories = Math.Round(calories,2);
            var calfat = (0.25) * ddal.calories;
            ddal.fat = Math.Round(calfat / 9,2);
            ddal.protein = Math.Round((1.5) * weight,2);
            var calprotein = ddal.protein * 4;
            var calfromproteinandfat = calfat + calprotein;
            var calcarb = ddal.calories - (calfromproteinandfat);
            ddal.carbohydrate = Math.Round(calcarb / 4,2);
            ddal.maintenance = Math.Round(maintenance,2);

            ddal.calfromprotein = Math.Round(ddal.protein * 4,2);
            ddal.calfromcarb = Math.Round(ddal.carbohydrate * 4,2);
            ddal.calfromfat = Math.Round(ddal.fat * 9,2);

            ddal.perfromprotein = Math.Round(((ddal.calfromprotein) / (ddal.calories)) * 100,2);
            ddal.perfromcarb = Math.Round(((ddal.calfromcarb) / (ddal.calories)) * 100,2);
            ddal.perfromfat = Math.Round(((ddal.calfromfat) / (ddal.calories)) * 100,2);

            var heightinm = height * 0.01;
            ddal.bmi = Math.Round((weight) / (heightinm * heightinm),2);

            ddal.message = "Your fat% is ideal for your goal";
            return ddal;
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
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance - (0.2) * maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    else if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "b")
                        {

                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance + ((0.25) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat > 12 && cal.bodyfat < 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance - ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance + ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                            return ddal;
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat > 12 && cal.bodyfat < 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance - ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance + ((0.1) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                }
                else if (cal.lifestyle == "la")
                {
                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance - (0.2) * maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";  
                        }
                    }

                    else if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "b")
                        {

                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance + ((0.25) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;

                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat > 12 && cal.bodyfat < 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance - ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;

                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance + ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat > 12 && cal.bodyfat < 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance - ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance + ((0.1) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                }
                else if (cal.lifestyle == "ma")
                {
                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance - (0.2) * maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    else if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "b")
                        {

                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance + ((0.25) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat > 12 && cal.bodyfat < 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance - ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance + ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat > 12 && cal.bodyfat < 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance - ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance + ((0.1) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";  
                        }
                    }
                }

                else if (cal.lifestyle == "ha")
                {
                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance - (0.2) * maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    else if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "b")
                        {

                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance + ((0.25) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat > 12 && cal.bodyfat < 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance - ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance + ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat > 12 && cal.bodyfat < 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat >= 18)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance - ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat <= 12)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age + 5;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance + ((0.1) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                }
                return ddal;
            }
            //-----------------------------------------------------------------------FEMALE-------------------------------------------------
            else if (cal.gender == "female")
            {
                if (cal.lifestyle == "s")
                {
                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance - ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }


                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance + ((0.25) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat > 22 && cal.bodyfat < 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                    //intermediate

                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance + ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance + ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat > 22 && cal.bodyfat < 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                    //advanced


                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance - ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }


                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.3;
                                ddal.calories = maintenance + ((0.10) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                }

                else if (cal.lifestyle == "la")
                {
                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance - ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance + ((0.25) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat > 22 && cal.bodyfat < 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                    //intermediate


                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance + ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance + ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }



                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat > 22 && cal.bodyfat < 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";    
                        }
                    }
                    //advanced


                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance - ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }


                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.6;
                                ddal.calories = maintenance + ((0.10) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                }

                else if (cal.lifestyle == "ma")
                {
                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance - ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }


                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance + ((0.25) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat > 22 && cal.bodyfat < 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                    //intermediate


                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance + ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance + ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }



                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat > 22 && cal.bodyfat < 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                    //advanced


                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance - ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }


                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 1.8;
                                ddal.calories = maintenance + ((0.10) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                }

                else if (cal.lifestyle == "ha")
                {
                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance - ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }


                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance + ((0.25) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "b")
                        {
                            if (cal.bodyfat > 22 && cal.bodyfat < 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                    //intermediate


                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance + ((0.2) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }

                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance + ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }



                    if (cal.goal == "build muscle lose fat")
                    {
                        if (cal.experience == "i")
                        {
                            if (cal.bodyfat > 22 && cal.bodyfat < 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance;
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                    //advanced


                    if (cal.goal == "lose fat")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat >= 28)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance - ((0.15) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }


                    if (cal.goal == "build muscle")
                    {
                        if (cal.experience == "a")
                        {
                            if (cal.bodyfat <= 22)
                            {
                                bmr = 10 * cal.weight + 6.25 * cal.height - 5 * cal.age - 161;
                                maintenance = bmr * 2;
                                ddal.calories = maintenance + ((0.10) * maintenance);
                                var result = malediet(maintenance, bmr, cal.weight, cal.height, ddal.calories);
                                return result;
                            }
                            ddal.message = "Your goal is not ideal for your fat%, choose a different goal";
                        }
                    }
                }
            }
            //caloriecount res = dietgen(maintenance, cal.bodyfat, cal.gender, cal.experience, cal.goal,cal.weight);
            return ddal;
            //return new responsecal { bmr = bmr, maintenance = maintenance, status = "success" };
            //	return $"BMR = {bmr} and Maintenance Calories = {maintenance}";
        }

        public responsebmi bmi(calculatebmi cal)
		{
			string path;
			string data = "";
			var weight = cal.weight;
			var height = cal.height;
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
			if (data.Length > 0)
			{
				if (data == "Obese")
				{
					path = @"https://pdfhost.io/v/7x9x9DdXc_Microsoft_Word_OBESE_WORKOUTdocx";
				}

                else if (data == "Underweight")
                {
                    path = @"https://pdfhost.io/v/Lj~RSIpot_Microsoft_Word_UNDERWEIGHT_WORKOUTdocx";
                }
                else if (data == "Normal")
                {
                    path = @"https://pdfhost.io/v/8qZdymY2._Microsoft_Word_NORMAL_WORKOUTdocx";
                }
                else if(data == "Overweight"){
                    path = @"https://pdfhost.io/v/kUCqJA.2Z_Microsoft_Word_OVERWEIGHT_WORKOUTdocx";
                }
				else 
				{
					path = @"https://pdfhost.io/v/~YGYgFkTW_Microsoft_Word_EXCEEDINGLY_OVERWEIGHT_WORKOUTdocx";
				}
				return new responsebmi { isSuccess= true, bmi = index, result = data, fileurl = path };
			}
			return new responsebmi { isSuccess = false };
		}
	}
}
