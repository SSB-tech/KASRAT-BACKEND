using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Claims;

namespace Kasrat.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class CalculationController : ControllerBase
    {
        dal calcu;
        IConfiguration configuration;

        public CalculationController(dal calcu, IConfiguration configuration)
        {
            this.calcu = calcu;
            this.configuration = configuration;
        }

        [HttpPost, Authorize]
        [Route("api/[controller]/bmr")]  //height in cm
        public ActionResult Get(calculate cal)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            caloriecount res = calcu.dietplan(cal);

            using (var conn = new SqlConnection(configuration.GetConnectionString("default")))
            {
                conn.Open();
                conn.Execute("INSERT INTO progress (UserID,Weight,BodyFat,BMI) VALUES (@UserID,@Weight,@BodyFat,@BMI)", new { UserID = userId, Weight = cal.weight, BodyFat = cal.bodyfat, BMI = res.bmi });
            }
            return Ok(res);
            //return new OkObjectResult(new responsecal { bmr = res.bmr, maintenance = res.maintenance });
        
        }

        [HttpPost, Authorize]
        [Route("api/[controller]/bmi")]
        public responsebmi bmi(calculatebmi cal)     //height in meter, weight in kg
        {
            var bodymassindex = calcu.bmi(cal);
            return bodymassindex;
        }

        [HttpPost, Authorize]
        [Route("api/[controller]/progress")]
        public async Task<ActionResult<List<progress>>> progresstrack()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var conn = new SqlConnection(configuration.GetConnectionString("default"));
            var data = await conn.QueryAsync<progress>("SELECT Date, Weight, BMI,BodyFat FROM progress WHERE UserID=@UserID", new { UserID = userId });
            return Ok(data.ToList());
        }



    }
}
