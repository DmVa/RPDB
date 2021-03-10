using RPDB.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RPDB.Services
{
    public class ScriptRunner
    {
        private Dictionary<int, Database> _dataBaseInfo;
        private ServerSetting _serverSetting;

        public void RegisterWithoutRun(ScriptData script, int DatabaseId)
        {
            if (DatabaseId == 0)
                throw new ApplicationException("database not defined");

            using (var dbcontext = new DataContext())
            {
                var registration = RegisterScript(script, DatabaseId, dbcontext);
                dbcontext.SaveChanges();
                script.Status = ScriptStatus.Unchanged;
                script.Registered = registration;
            }
        }

        private Data.RegisteredScript RegisterScript(ScriptData script, int DatabaseId, DataContext dbcontext)
        {
            Data.RegisteredScript registration = null;
            if (script.Registered == null)
            {
                registration = new Data.RegisteredScript();
                dbcontext.RegisteredScripts.Add(registration);
                //registration.
            }
            else
            {
                registration = dbcontext.RegisteredScripts.FirstOrDefault(x => x.Id == script.Registered.Id);
            }
            registration.FileSize = Convert.ToInt32(script.FileData.FileSize);
            registration.FileTime = script.FileData.FileDate;
            registration.Name = script.FileData.FileName;
            registration.Path = script.FileData.Path;
            registration.DatabaseId = DatabaseId;
            registration.Text = File.ReadAllText(script.FileData.FullFileName);
            return registration;
        }

        internal void RegisterAllWithoutRun(ObservableCollection<ScriptData> scripts)
        {
            using (var dbcontext = new DataContext())
            {
                foreach (var script in scripts)
                {
                    var registration = RegisterScript(script,script.ExpectedDatabaseId, dbcontext);
                    dbcontext.SaveChanges();
                    script.Status = ScriptStatus.Unchanged;
                    script.Registered = registration;
                }
            }
        }

        internal void UnRegisterAllScripts()
        {
            using (var dbcontext = new DataContext())
            {
                dbcontext.RegisteredScripts.RemoveRange(dbcontext.RegisteredScripts);
                dbcontext.SaveChanges();
            }
        }

        internal void RunScript(ScriptData script, int databaseId, List<string> warrings, List<string> errors)
        {
            if (script == null)
                throw new ApplicationException("Script not selected");

            if (databaseId == 0)
                throw new ApplicationException("database not defined");

            EnsureDbDataLoaded();
            if (!_dataBaseInfo.ContainsKey(databaseId))
                 throw new ApplicationException($"database info not loaded for selected database"); ;

            string sql = File.ReadAllText(script.FileData.FullFileName);

            if (string.IsNullOrEmpty(sql))
                throw new ApplicationException($"SQL not loaded from {script?.FileData?.FullFileName ?? "null"}");

            string connectionString = GetConnectionString(databaseId);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException($"cannot build connection string to database {_dataBaseInfo[databaseId].Name} server: {_serverSetting.Name}");
            }

			IEnumerable<string> statements = SplitSqlStatements(sql);
			VerifyStatements(statements, warrings, errors);
			if (errors.Count > 0)
				throw new ApplicationException(errors[0]);

			using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (string statement in statements)
                {
                    string statementToExecute = ReplaceTokens(statement);
                    ExecuteSqlCommand(statementToExecute, conn);
                }
            }

            using (var dbcontext = new DataContext())
            {
                var registration = RegisterScript(script, databaseId, dbcontext);
                registration.Executed = true;
                registration.LastExecutionError = null;
                registration.ExecutedTime = DateTime.Now;
                dbcontext.SaveChanges();
                script.Status = ScriptStatus.Unchanged;
                script.Registered = registration;
            }
        }

		private void VerifyStatements(IEnumerable<string> statements, List<string> warrnings, List<string> errors)
		{
			if (warrnings == null)
				warrnings = new List<string>();
			if (errors == null)
				errors = new List<string>();
			foreach (string statement in statements)
			{
				if (statement.ToUpper().Contains("USE "))
				{
					warrnings.Add("USE statement not supported. Statement ignored.");
				}
			}
		}

		private void EnsureDbDataLoaded()
        {
            using (var dbcontext = new DataContext())
            {
                if (_serverSetting == null)
                {
                    _serverSetting = dbcontext.ServerSettings.FirstOrDefault();
                }


                if (_dataBaseInfo == null)
                {
                    _dataBaseInfo = dbcontext.Databases.ToDictionary(x => x.Id);
                }
            }

            if (_serverSetting == null)
                throw new ApplicationException("server settings not defined");

            if (_dataBaseInfo == null || _dataBaseInfo.Count == 0)
            {
                throw new ApplicationException("Databases not defined");
            }

        }

        private string GetConnectionString(int databaseId)
        {
            if (!_dataBaseInfo.ContainsKey(databaseId))
                return null;

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = _serverSetting.Name;

            if (_serverSetting.IsWindowsAuth)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.IntegratedSecurity = true;
                builder.UserID = _serverSetting.UserName;
                builder.Password = _serverSetting.Password;
            }
            builder.InitialCatalog = _dataBaseInfo[databaseId].Name;
            return builder.ConnectionString;
        }

        private string ReplaceTokens(string statement)
        {
            foreach(var db in _dataBaseInfo)
            {
                if (!string.IsNullOrEmpty(db.Value.Alias))
                    statement = ReplaceCaseInsensitive(statement, db.Value.Alias, db.Value.Name);
            }
            return statement;
        }

        private static string ReplaceCaseInsensitive(string input, string search, string replacement)
        {
            string result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }

        private void ExecuteSqlCommand(string statement, SqlConnection connection)
        {
            SqlTransaction trans = null;

            try
            {
                trans = connection.BeginTransaction();

                SqlCommand command = new SqlCommand(statement, connection, trans);
                command.CommandTimeout = 2 * 60;
                command.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception) 
            {
                if (trans != null)
                    trans.Rollback();

                throw;
            }
            finally
            {
                if (trans != null)
                    trans.Dispose();
            }
        }

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Split by "GO" statements
            var statements = Regex.Split(
                    sqlScript,
                    @"^\s*GO\s* ($ | \-\- .*$)",
                    RegexOptions.Multiline |
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.IgnoreCase);

            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'));
        }
    }
}
