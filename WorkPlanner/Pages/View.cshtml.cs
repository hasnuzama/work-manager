using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WorkPlanner.DAL;
using WorkPlanner.Models;

namespace WorkPlanner
{
	public class ViewModel : PageModel
	{
		[BindProperty]
		public List<WorkPlan> WorkPlans { get; set; }
		[BindProperty]
		public List<User> Users { get; set; }

		public void OnGet()
		{
			string role = HttpContext.Session.GetString("Role") ?? string.Empty;
			BindUsers(role);
		}

		#region Not required as of now

		public IActionResult OnGetUsers()
		{
			WPResponse response = new WPResponse(ResponseStatus.Error);
			int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
			if(userId <= 0)
			{
				response.ErrorCode = ErrorCode.SESSION_NOT_FOUND;
				new JsonResult(JsonConvert.SerializeObject(response));
			}
			string role = HttpContext.Session.GetString("Role") ?? string.Empty;
			if(!role.Equals("admin"))
			{
				response.ErrorCode = ErrorCode.UNAUTHORIZED;
				new JsonResult(JsonConvert.SerializeObject(response));
			}
			response.Results = Database.GetAllUsers().Cast<BaseDBModel>().ToList();
			response.Status = ResponseStatus.Success;
			return new JsonResult(JsonConvert.SerializeObject(response));
		}

		#endregion

		public IActionResult OnGetWorkPlans(string userId, string date)
		{
			WPResponse response = new WPResponse(ResponseStatus.Error);
			int intUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
			string role = HttpContext.Session.GetString("Role");
			// Validate authentication
			if (intUserId <= 0 || string.IsNullOrWhiteSpace(role))
			{
				response.ErrorCode = ErrorCode.SESSION_NOT_FOUND;
				return new JsonResult(JsonConvert.SerializeObject(response));
			}
			if (role.Equals("admin") &&
				!string.IsNullOrWhiteSpace(userId))
			{
				intUserId = Convert.ToInt32(userId);
			}

			// Validation
			if (intUserId <= 0 ||
				string.IsNullOrWhiteSpace(date))
			{
				response.ErrorCode = ErrorCode.INVALID_DATA;
				return new JsonResult(JsonConvert.SerializeObject(response));
			}
			DateTime dt;
			var formatStrings = new string[] { "yyyy-MM-dd", "yyyy-MM-d", "yyyy-M-dd", "yyyy-M-d" };
			if (!DateTime.TryParseExact(date, formatStrings, null, DateTimeStyles.None, out dt))
			{
				response.ErrorCode = ErrorCode.INVALID_DATA;
				new JsonResult(JsonConvert.SerializeObject(response));
			}

			// Obtain data
			response.Results = Database.GetWorkplans(intUserId, dt).Cast<BaseDBModel>().ToList();
			response.Status = ResponseStatus.Success;
			return new JsonResult(JsonConvert.SerializeObject(response));
		}

		private void BindUsers(string role)
		{
			if (role.Equals("admin"))
			{
				Users = Database.GetAllUsers();
			}
		}
	}
}