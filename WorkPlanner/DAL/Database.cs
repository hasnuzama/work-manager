using System;
using System.Collections.Generic;
using System.Globalization;
using MySql.Data.MySqlClient;
using WorkPlanner.Helpers;
using WorkPlanner.Models;

namespace WorkPlanner.DAL
{
	public class Database
	{
		private static string _connectionString = "Server=localhost;Port=3306;Database=work_manager;Uid=root;Pwd=tech;";

		public Database()
		{
			// TODO : Deprecate this after implementing the functionality of reading connection string from appsettings.
			// TODO : Make methods as non static.
		}

		public Database(string connectionString)
		{
			_connectionString = connectionString;
		}

		public static User GetUser(string email, string password)
		{
			MySqlConnection conn = new MySqlConnection(_connectionString);
			User objResult = null;
			try
			{
				MySqlCommand command = new MySqlCommand("select user_id, role from users where email=@Email and `password`=@Password", conn);
				command.Parameters.AddWithValue("Email", email);
				command.Parameters.AddWithValue("Password", password);
				conn.Open();
				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					objResult = new User();
					objResult.UserId = (UInt32)reader["user_id"];
					objResult.Role = Convert.ToString(reader["role"]);
					return objResult;
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				conn.Close();
			}
			return objResult;
		}

		public static List<User> GetAllUsers()
		{
			MySqlConnection conn = new MySqlConnection(_connectionString);
			List<User> objResults = null;
			User objResult = null;
			try
			{
				MySqlCommand command = new MySqlCommand("select user_id, name from users order by name", conn);
				conn.Open();
				var reader = command.ExecuteReader();
				objResults = new List<User>();
				while (reader.Read())
				{
					objResult = new User();
					objResult.UserId = (UInt32)reader["user_id"];
					objResult.Name = Convert.ToString(reader["name"]);
					objResults.Add(objResult);
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				conn.Close();
			}
			return objResults;
		}

		public static int InsertWorkPlans(IList<WorkPlan> workPlans)
		{
			MySqlConnection conn = new MySqlConnection(_connectionString);
			MySqlCommand command = null;
			MySqlTransaction myTrans = null;
			int count = 0;
			try
			{
				conn.Open();
				myTrans = conn.BeginTransaction();
				command = new MySqlCommand();
				command.Transaction = myTrans;
				DateTime currentTime = DateTime.Now;
				for (int i = 0; i < workPlans.Count; i++)
				{
					command.CommandText = $@"insert into
							work_plans(user_id,work_date,task_name,project_name,estimated_hours,pinestem_task_id,created_on)
						values
							(@UserId,@WorkDate,@TaskName,@ProjectName,@EstimatedHours,@PineStemTaskId, @CreatedOn)";
					command.Parameters.Clear();
					command.Parameters.AddWithValue("UserId", workPlans[i].UserId);
					command.Parameters.AddWithValue("WorkDate", workPlans[i].WorkDate);
					command.Parameters.AddWithValue("TaskName", workPlans[i].TaskDetails);
					command.Parameters.AddWithValue("ProjectName", workPlans[i].ProjectName);
					command.Parameters.AddWithValue("EstimatedHours", workPlans[i].EstimatedHours);
					command.Parameters.AddWithValue("PineStemTaskId", workPlans[i].PineStemTaskID);
					command.Parameters.AddWithValue("CreatedOn", currentTime);
					command.Connection = conn;
					count += command.ExecuteNonQuery();
				}
				myTrans.Commit();
			}
			catch (Exception)
			{
				myTrans.Rollback();
			}
			finally
			{
				conn.Close();
			}
			return count;
		}

		public static List<WorkPlan> GetWorkplans(string[] userId, DateTime workDate, int intUserId)
		{
			MySqlConnection conn = new MySqlConnection(_connectionString);
			List<WorkPlan> objResults = null;
			try
			{
				    objResults = new List<WorkPlan>();
				   WorkPlan objResult = null;
					var ID = userId[0];
					var dd = workDate.Date.Day;
					var mm = workDate.Date.Month;
					var yyyy = workDate.Date.Year;
					var _date = yyyy + "-" + mm + "-" + dd;
				if (ID == null && intUserId!=0)
				{
					
					

					string Query = "SELECT users.name, wp.task_name,wp.project_name,wp.estimated_hours,wp.pinestem_task_id,wp.created_on FROM work_plans AS wp LEFT JOIN users AS users ON users.user_id = wp.user_id where wp.user_id in (" + intUserId + ")  and  wp.work_date BETWEEN    '" + _date + "' AND '" + _date + "'";
					MySqlCommand command = new MySqlCommand(Query, conn);
					//command.Parameters.AddWithValue("UserId", i);
					command.Parameters.AddWithValue("WorkDate", workDate.Date);
					conn.Open();
					var reader = command.ExecuteReader();

					while (reader.Read())
					{
						objResult = new WorkPlan();
						objResult.EmployeeName = Convert.ToString(reader["name"]);
						objResult.TaskDetails = Convert.ToString(reader["task_name"]);
						objResult.ProjectName = Convert.ToString(reader["project_name"]);
						objResult.EstimatedHours = Convert.ToString(reader["estimated_hours"]);
						objResult.PineStemTaskID = Convert.ToString(reader["pinestem_task_id"]);
						objResult.CreatedOn = Convert.ToDateTime(reader["created_on"]);
						objResults.Add(objResult);
					}

				}

				else
				{
					

					string Query = "SELECT users.name, wp.task_name,wp.project_name,wp.estimated_hours,wp.pinestem_task_id,wp.created_on FROM work_plans AS wp LEFT JOIN users AS users ON users.user_id = wp.user_id where wp.user_id in (" + ID + ")  and  wp.work_date BETWEEN    '" + _date + "' AND '" + _date + "'";
					MySqlCommand command = new MySqlCommand(Query, conn);
					//command.Parameters.AddWithValue("UserId", i);
					command.Parameters.AddWithValue("WorkDate", workDate.Date);
					conn.Open();
					var reader = command.ExecuteReader();

					while (reader.Read())
					{
						objResult = new WorkPlan();
						objResult.EmployeeName = Convert.ToString(reader["name"]);
						objResult.TaskDetails = Convert.ToString(reader["task_name"]);
						objResult.ProjectName = Convert.ToString(reader["project_name"]);
						objResult.EstimatedHours = Convert.ToString(reader["estimated_hours"]);
						objResult.PineStemTaskID = Convert.ToString(reader["pinestem_task_id"]);
						objResult.CreatedOn = Convert.ToDateTime(reader["created_on"]);
						objResults.Add(objResult);
					}

				}
				  			
			     
				
				
			}
			catch (Exception ex)
			{
			}
			finally
			{
				conn.Close();
			}
			return objResults;
		}
	}
}
