using System;
using System.Collections;
using System.Collections.Generic;
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
	public class CreateModel : PageModel
	{
		public void OnGet()
		{
		}

		public IActionResult OnPostWorkPlan([FromBody] List<WorkPlan> workPlans)
		{
			WPResponse response = new WPResponse(ResponseStatus.Error);

			// Validate
			if (workPlans == null || workPlans.Count == 0)
			{
				response.ErrorCode = ErrorCode.INVALID_DATA;
				return new JsonResult(JsonConvert.SerializeObject(response));
			}
			int? userId = HttpContext.Session.GetInt32("UserId");
			if (userId == null || userId == 0)
			{
				response.ErrorCode = ErrorCode.SESSION_NOT_FOUND;
				return new JsonResult(JsonConvert.SerializeObject(response));
			}

			// Preprocess
			for (int i = 0; i < workPlans.Count; i++)
			{
				workPlans[i].UserId = (UInt32)userId;
				workPlans[i].ProjectName = (workPlans[i].ProjectName ?? string.Empty).Trim();
				workPlans[i].TaskDetails = (workPlans[i].TaskDetails ?? string.Empty).Trim();
				workPlans[i].EstimatedHours = (workPlans[i].EstimatedHours ?? string.Empty).Trim();
				if(workPlans[i].WorkDate == DateTime.MinValue)
				{
					workPlans[i].WorkDate = DateTime.Now.Date;
				}
			}

			// Insert
			var count = Database.InsertWorkPlans(workPlans);
			if (count == workPlans.Count)
			{
				response.Status = ResponseStatus.Success;
				return new JsonResult(JsonConvert.SerializeObject(response));
			}
			response.ErrorCode = ErrorCode.DATABASE_VALIDATION_FAILED;
			return new JsonResult(JsonConvert.SerializeObject(response));
		}
	}
}