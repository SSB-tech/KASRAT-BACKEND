using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kasrat
{
	public class register
	{
		public string? UserName { get; set; }

        [RegularExpression(@"^.{6,}$", ErrorMessage = "Password must be at least 6 characters long")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword {get; set;}
		public string? Email { get; set; }
	}
	public class response
	{
		public bool isSuccess { get; set; }
		public string message { get; set; }
		public string token { get; set; }

	}
	public class login
	{
		public string? username { get; set; }
		public string? password { get; set; }
	}

	public class calculate
	{
		public double weight { get; set; } 
		public double height { get; set; }
		public int age { get; set; }
		public string lifestyle { get; set; }
		public string gender { get; set; }
		public float bodyfat { get; set; }
		public string experience { get; set;}
		public string goal { get; set;}
	
	}

	public class caloriecount
	{
		public double calories { get; set;}
		public double carbohydrate { get; set;}
		public double protein { get; set; }
		public double fat { get; set; }
		public double maintenance { get; set; }
		public double calfromcarb { get; set; }
		public double calfromprotein { get; set; }
		public double calfromfat { get; set; }
        public double perfromcarb { get; set; }
        public double perfromprotein { get; set; }
        public double perfromfat { get; set; }
		public double bmi { get; set; }
		public string message { get; set;}
	}

	public class progress
	{
		public DateTime date { get; set; }
        public double bmi { get; set; }
		public double weight { get; set; }
		public double bodyfat { get; set; }
	}

    public class calculatebmi
	{
		public double weight { get; set; }
		public double height { get; set; }
	} 

	public class responsecal
	{
		public string status { get; set; }
		public double bmr { get; set; }
		public double maintenance { get; set; }
	}

	public class responsebmi
	{
        public bool isSuccess { get; set; }

        public double bmi { get; set; }
		public string result { get; set; }

		public string fileurl { get; set; }
		//public FileStreamResult file { get; set; }
	}

	public class recommend
	{
		public int healthcondition { get; set; }
		public int fitnessgoal { get; set; }

	}
}
