using NLog;
using ProjectTracker.SolutionExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.DAL.Sql
{
    public class Tasks : ITasks
    {
        private const string entityName = "Tasks";
        private static Logger _log = LogManager.GetCurrentClassLogger();

        private readonly string connectionString;

        internal Tasks(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /*  Audit Logs are wrapped in try catch to keep things running if an error when logging occurs  */


        public async Task<TResult> InsertAsync<TResult>(string name, string description, bool isComplete, string username, Func<long, TResult> success, Func<string, TResult> failed, Func<TResult> unAuthorized)
        {
            using (var internalConnection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;

                isComplete = false;

                try
                {
                    await internalConnection.OpenAsync();
                    transaction = internalConnection.BeginTransaction();

                    var query =
                        "INSERT INTO Tasks([Name], [Description], [IsComplete]) VALUES (@Name, @Description, @IsComplete); SELECT SCOPE_IDENTITY()";

                    var cmd = new SqlCommand(string.Empty, internalConnection) { CommandText = query };
                    cmd.Parameters.Clear();
                    cmd.Transaction = transaction;

                    cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = name });
                    cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { Value = description });
                    cmd.Parameters.Add(new SqlParameter("@IsComplete", SqlDbType.Bit) { Value = isComplete });

                    var rowsItems = await cmd.ExecuteScalarAsync(); var rows = Convert.ToInt64(rowsItems);

                    try
                    {
                        await transaction.LogInsertAsync(entityName, "Name", rows, name, username);
                        await transaction.LogInsertAsync(entityName, "Description", rows, description, username);
                        await transaction.LogInsertAsync(entityName, "IsComplete", rows, isComplete.ToString(), username);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, ex.Message, ex.StackTrace);

                    }

                    transaction.Commit();

                    return rows > 0 ? success(rows) : failed("No rows saved. Error occurred");
                }
                catch (SqlException sqlEx)
                {
                    transaction?.Rollback();
                    _log.Error(sqlEx, sqlEx.Message, sqlEx.StackTrace);
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    _log.Error(ex, ex.Message, ex.StackTrace);
                }
                return unAuthorized();
            }
        }

        public async Task<TResult> UpdateAsync<TResult>(long id, string name, string description, bool isComplete, string username,
            Func<TResult> success,
            Func<string, TResult> failed,
            Func<TResult> unAuthorized)
        {
            using (var internalConnection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;

                try
                {
                    await internalConnection.OpenAsync();
                    transaction = internalConnection.BeginTransaction();

                    var query = "UPDATE [dbo].[Tasks] SET" +
                                "[Name] = @Name," +
                                "[Description] = @Description," +
                                "[IsComplete] = @IsComplete WHERE Id = @Id";


                    var cmd = new SqlCommand(string.Empty, internalConnection) { CommandText = query };
                    cmd.Parameters.Clear();
                    cmd.Transaction = transaction;

                    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.BigInt) { Value = id });
                    cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = name });
                    cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { Value = description });
                    cmd.Parameters.Add(new SqlParameter("@IsComplete", SqlDbType.Bit) { Value = isComplete });

                    var rowsItems = await cmd.ExecuteNonQueryAsync(); var rows = Convert.ToInt64(rowsItems);
                    try
                    {
                        var oldName = default(string);
                        var oldDescription = default(string);
                        var oldIsComplete = default(bool);
                        await FindTaskByIdAsync(id, (s, description1, u) =>
                        {
                            oldName = s;
                            oldDescription = description1;
                            oldIsComplete = u;
                            return true;
                        }, () => true, () => false);

                        await transaction.LogUpdateAsync(entityName, "Name", id, oldName, name, username);
                        await transaction.LogUpdateAsync(entityName, "Description", id, oldDescription, description, username);
                        await transaction.LogUpdateAsync(entityName, "IsComplete", id, oldIsComplete.ToString(), isComplete.ToString(), username);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, ex.Message, ex.StackTrace);
                    }

                    transaction.Commit();

                    return rows > 0 ? success() : failed("No rows saved. Error occurred");
                }
                catch (SqlException sqlEx)
                {
                    transaction?.Rollback();
                    _log.Error(sqlEx, sqlEx.Message, sqlEx.StackTrace);
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    _log.Error(ex, ex.Message, ex.StackTrace);
                }
                return unAuthorized();
            }
        }

        public async Task<TResult> DeleteAsync<TResult>(long id, string username, Func<TResult> success, Func<string, TResult> failed)
        {
            using (var internalConnection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;

                try
                {
                    await internalConnection.OpenAsync(); ;
                    transaction = internalConnection.BeginTransaction();
                    var query = "Delete From Tasks where Id = @Id";

                    var cmd = new SqlCommand(string.Empty, internalConnection) { CommandText = query };
                    cmd.Parameters.Clear();
                    cmd.Transaction = transaction;

                    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.BigInt) { Value = id });

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();

                    try
                    {
                        await transaction.LogDeleteAsync(entityName, id, username);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, ex.Message, ex.StackTrace);
                    }

                    transaction.Commit();
                    return rowsAffected > 0 ? success() : failed("Item not found, or error occurred.");
                }
                catch (SqlException sqlEx)
                {
                    transaction?.Rollback();
                    _log.Error(sqlEx, sqlEx.Message, sqlEx.StackTrace);
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    _log.Error(ex, ex.Message, ex.StackTrace);
                }
                return default(TResult);
            }
        }

        public async Task<TResult> GetAllTasksAsync<TResult>(TasksInfoDelegateAsync<TResult> callback,
            Func<TResult> done, Func<TResult> notFound)
        {
            using (var internalConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await internalConnection.OpenAsync();

                    var query = "SELECT Id, Name, Description, IsComplete FROM Tasks";

                    var cmd = new SqlCommand(string.Empty, internalConnection) { CommandText = query };
                    cmd.Parameters.Clear();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows) return notFound();
                        while (reader.Read())
                        {

                            callback(reader["Id"].ToLong(),
                                reader["Name"].ToString(),
                                reader["Description"].ToString(),
                                reader["IsComplete"].ToBool());
                        }
                    }

                    return done();
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, sqlEx.Message, sqlEx.StackTrace);
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message, ex.StackTrace);
                }
                return notFound();
            }
        }

        public async Task<TResult> FindTaskByIdAsync<TResult>(long id, TaskInfoDelegateAsync<TResult> callback,
            Func<TResult> done, Func<TResult> notFound)
        {
            using (var internalConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await internalConnection.OpenAsync();

                    var query = "SELECT Id, Name, Description, IsComplete FROM Tasks WHERE Id = @Id";

                    var cmd = new SqlCommand(string.Empty, internalConnection) { CommandText = query };
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.BigInt) { Value = id });

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows) return notFound();
                        while (reader.Read())
                        {
                            callback(
                                reader["Name"].ToString(),
                                reader["Description"].ToString(),
                                reader["IsComplete"].ToBool());
                        }
                    }

                    return done();
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, sqlEx.Message, sqlEx.StackTrace);
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message, ex.StackTrace);
                }
                return notFound();
            }
        }

    }
}
