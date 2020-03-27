using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using WorkPlanner.Models;

namespace WorkPlanner.DAL
{
	public class Database
	{
		private static string _connectionString = "Server=localhost;Port=3306;Database=work_manager;Uid=root;Pwd=ha;";

		public static User GetUser(string email, string password)
		{
			MySqlConnection conn = new MySqlConnection(_connectionString);
			User objResult = null;
			try
			{
				MySqlCommand command = new MySqlCommand("select user_id, role from users where email=@Email and `password`=@Password", conn);
				command.Parameters.AddWithValue("Email", email);
				command.Parameters.AddWithValue("Password", CalculatePasswordHash(password));
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
				for (int i = 0; i < workPlans.Count; i++)
				{
					command.CommandText = $@"insert into
							work_plans(user_id,work_date,task_name,project_name,estimated_hours,pinestem_task_id)
						values
							(@UserId,@WorkDate,@TaskName,@ProjectName,@EstimatedHours,@PineStemTaskId)";
					command.Parameters.Clear();
					command.Parameters.AddWithValue("UserId", workPlans[i].UserId);
					command.Parameters.AddWithValue("WorkDate", workPlans[i].WorkDate);
					command.Parameters.AddWithValue("TaskName", workPlans[i].TaskDetails);
					command.Parameters.AddWithValue("ProjectName", workPlans[i].ProjectName);
					command.Parameters.AddWithValue("EstimatedHours", workPlans[i].EstimatedHours);
					command.Parameters.AddWithValue("PineStemTaskId", workPlans[i].PineStemTaskID);
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
				MySqlCommand command = new MySqlCommand("select task_name, project_name, estimated_hours, pinestem_task_id from work_plans where user_id=@UserId and work_date=@WorkDate", conn);
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

		private static string CalculatePasswordHash(string input)
		{
			input = input ?? string.Empty;
			// step 1, calculate MD5 hash from input
			MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}
	}
}
