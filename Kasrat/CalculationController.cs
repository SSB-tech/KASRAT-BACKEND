using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kasrat
{
	//[Route("api/[controller]")]
	[ApiController]
	public class CalculationController : ControllerBase
	{
		dal calcu;

		public CalculationController(dal calcu)
		{
			this.calcu = calcu;
		}

		[HttpPost]
		[Route("api/[controller]/bmr")]  //height in cm
		public ActionResult Get(calculate cal)
		{
			caloriecount res = calcu.dietplan(cal);
			return Ok(res);
			//return new OkObjectResult(new responsecal { bmr = res.bmr, maintenance = res.maintenance });
		}

		[HttpPost]
		[Route("api/[controller]/bmi")]
		public responsebmi bmi(calculatebmi cal)     //height in meter, weight in kg
		{
			var bodymassindex = calcu.bmi(cal);
			return bodymassindex;
		}

	}
}
