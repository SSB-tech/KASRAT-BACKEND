using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Kasrat
{
	//[Route("api/[controller]")]
	[ApiController]
	public class StateController : ControllerBase
	{
		public dal _conn;

		IConfiguration configuration;

		public StateController(IConfiguration configuration, dal conn)
		{
			this.configuration = configuration;
			_conn = conn;
		}

		[HttpPost]
		[Route("api/[controller]/register")]
		public IActionResult registeration(model a)
		{
			var con = new SqlConnection(configuration.GetConnectionString("default"));
			var data = _conn.register(a);
			return Ok(data);
		}

		[HttpPost]
		[Route("api/[controller]/login")]
		public async Task<IActionResult> login(login a)
		{
			var data = _conn.login(a);
			return Ok(data);
		}

	}
}

