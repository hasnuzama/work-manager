using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WorkPlanner.DAL;
using WorkPlanner.Models;

namespace WorkPlanner.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;

		public IndexModel(ILogger<IndexModel> logger)
		{
			_logger = logger;
		}

		public void OnGet()
		{
		}

		public IActionResult OnPostLogin([FromBody] User userDetails)
		{
			WPResponse response = new WPResponse(ResponseStatus.Error);
			var email = userDetails.Email;
			var password = userDetails.Password;
			// Validate
			if(string.IsNullOrWhiteSpace(email) ||
				string.IsNullOrWhiteSpace(password))
			{
				response.ErrorCode = ErrorCode.INVALID_DATA;
				return new JsonResult(JsonConvert.SerializeObject(response));
			}
			// Verify and start the session
			User objUser = Database.GetUser(email, password);
			if (objUser != null)
			{
				HttpContext.Session.SetString("Role", objUser.Role);
				HttpContext.Session.SetInt32("UserId", Convert.ToInt32(objUser.UserId));
				response.Status = ResponseStatus.Success;
			}
			else
			{
				response.ErrorCode = ErrorCode.INVALID_CREDENTIAlS;
			}
			return new JsonResult(JsonConvert.SerializeObject(response));
		}
	}
}
