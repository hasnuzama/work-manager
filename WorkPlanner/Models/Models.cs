using System;
using System.Collections.Generic;

namespace WorkPlanner.Models
{
	public class BaseDBModel
	{
	}

	public class User : BaseDBModel
	{
		public UInt32 UserId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
	}

	public class WorkPlan : BaseDBModel
	{
		public WorkPlan()
		{
		}
		public string ProjectName { get; set; }
		public string TaskDetails { get; set; }
		public string EstimatedHours { get; set; }
		public string PineStemTaskID { get; set; }
		public UInt32 UserId { get; set; }
		public DateTime WorkDate { get; set; }
	}

	public class WPResponse
	{
		public WPResponse()
		{
		}

		public WPResponse(ResponseStatus status)
		{
			this.Status = status;
		}

		public ResponseStatus Status { get; set; }
		public ErrorCode ErrorCode { get; set; }
		public List<BaseDBModel> Results { get; set; }
	}

	public enum ResponseStatus
	{
		Success = 1,
		Error = 0
	}

	public enum ErrorCode
	{
		INVALID_DATA = 1000,
		INVALID_CREDENTIAlS = 1001,
		DATABASE_VALIDATION_FAILED = 1002,
		SESSION_NOT_FOUND = 1003,
		UNAUTHORIZED = 1004
	}
}
