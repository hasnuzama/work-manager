
using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using WorkPlanner.Helpers;
using WorkPlanner.Models;

namespace WorkPlanner.DAL
{
	public class Database
	{

		private static string _connectionString = null;

		public Database(string connection)
		{
			_connectionString = connection;
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

		public static List<WorkPlan> GetWorkplans(int userId, DateTime workDate)
		{
			MySqlConnection conn = new MySqlConnection(_connectionString);
			List<WorkPlan> objResults = null;
			try
			{
				objResults = new List<WorkPlan>();
				WorkPlan objResult = null;
				MySqlCommand command = new MySqlCommand("select task_name, project_name, estimated_hours, pinestem_task_id, created_on from work_plans where user_id=@UserId and work_date=@WorkDate", conn);
				command.Parameters.AddWithValue("UserId", userId);
				command.Parameters.AddWithValue("WorkDate", workDate.Date);
				conn.Open();
				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					objResult = new WorkPlan();
					objResult.TaskDetails = Convert.ToString(reader["task_name"]);
					objResult.ProjectName = Convert.ToString(reader["project_name"]);
					objResult.EstimatedHours = Convert.ToString(reader["estimated_hours"]);
					objResult.PineStemTaskID = Convert.ToString(reader["pinestem_task_id"]);
					objResult.CreatedOn = Convert.ToDateTime(reader["created_on"]);
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
	}
}
