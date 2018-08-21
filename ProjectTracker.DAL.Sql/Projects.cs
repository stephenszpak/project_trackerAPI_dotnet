using NLog;
using ProjectTracker.SolutionExtensions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ProjectTracker.DAL.Sql
{
    public class Projects : IProjects
    {
        private const string entityName = "Projects";
        private static Logger _log = LogManager.GetCurrentClassLogger();

        private readonly string connectionString;

        internal Projects(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<TResult> InsertAsync<TResult>(string name, string description, string projectSponsor,
            string executiveSponsor, string productSponsor, long projectTypeId, bool newPricingRules, long volume,
            decimal revenueAtList, bool dealFormEligible, string newTitles, string newAccounts, string projectDetails,
            string businessCase, string comments, DateTime createdDate, string username,
            Func<long, TResult> success,
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

                    var query = "INSERT INTO Projects([Name],[Description],[ProjectSponsor],[ExecutiveSponsor],[ProductSponsor],[ProjectTypeId],[NewPricingRules]," +
                        "[Volume],[RevenueAtList],[DealFormEligible],[NewTitles],[NewAccounts],[ProjectDetails],[BusinessCase],[Comments],[CreatedDate]) " +
                        "VALUES (@Name, @Description, @ProjectSponsor, @ExecutiveSponsor, @ProductSponsor, @ProjectTypeId, @NewPricingRules, " +
                        "@Volume, @RevenueAtList, @DealFormEligible, @NewTitles, @NewAccounts, @ProjectDetails, @BusinessCase, @Comments, @CreatedDate); " +
                        "SELECT SCOPE_IDENTITY()";

                    var cmd = new SqlCommand(string.Empty, internalConnection) { CommandText = query };
                    cmd.Parameters.Clear();
                    cmd.Transaction = transaction;

                    cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = name });
                    cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { Value = description });
                    cmd.Parameters.Add(new SqlParameter("@ProjectSponsor", SqlDbType.VarChar) { Value = projectSponsor });
                    cmd.Parameters.Add(new SqlParameter("@ExecutiveSponsor", SqlDbType.VarChar) { Value = executiveSponsor });
                    cmd.Parameters.Add(new SqlParameter("@ProductSponsor", SqlDbType.VarChar) { Value = productSponsor });
                    cmd.Parameters.Add(new SqlParameter("@ProjectTypeId", SqlDbType.Int) { Value = projectTypeId });
                    cmd.Parameters.Add(new SqlParameter("@NewPricingRules", SqlDbType.Bit) { Value = newPricingRules });
                    cmd.Parameters.Add(new SqlParameter("@Volume", SqlDbType.Int) { Value = volume });
                    cmd.Parameters.Add(new SqlParameter("@RevenueAtList", SqlDbType.Decimal) { Value = revenueAtList });
                    cmd.Parameters.Add(new SqlParameter("@DealFormEligible", SqlDbType.Bit) { Value = dealFormEligible });
                    cmd.Parameters.Add(new SqlParameter("@NewTitles", SqlDbType.VarChar) { Value = newTitles });
                    cmd.Parameters.Add(new SqlParameter("@NewAccounts", SqlDbType.VarChar) { Value = newAccounts });
                    cmd.Parameters.Add(new SqlParameter("@ProjectDetails", SqlDbType.VarChar) { Value = projectDetails });
                    cmd.Parameters.Add(new SqlParameter("@BusinessCase", SqlDbType.VarChar) { Value = businessCase });
                    cmd.Parameters.Add(new SqlParameter("@Comments", SqlDbType.VarChar) { Value = comments });
                    cmd.Parameters.Add(new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.Now });

                    var rowsItems = await cmd.ExecuteScalarAsync(); var rows = Convert.ToInt64(rowsItems);

                    try
                    {
                        await transaction.LogInsertAsync(entityName, "Name", rows, name, username);
                        await transaction.LogInsertAsync(entityName, "Description", rows, description, username);
                        await transaction.LogInsertAsync(entityName, "ProjectSponsor", rows, projectSponsor, username);
                        await transaction.LogInsertAsync(entityName, "ExecutiveSponsor", rows, executiveSponsor, username);
                        await transaction.LogInsertAsync(entityName, "ProductSponsor", rows, productSponsor, username);
                        await transaction.LogInsertAsync(entityName, "NewPricingRules", rows, newPricingRules.ToString(), username);
                        await transaction.LogInsertAsync(entityName, "Volume", rows, volume.ToString(), username);
                        await transaction.LogInsertAsync(entityName, "RevenueAtList", rows, revenueAtList.ToString(), username);
                        await transaction.LogInsertAsync(entityName, "DealFormEligible", rows, dealFormEligible.ToString(), username);
                        await transaction.LogInsertAsync(entityName, "NewTitles", rows, newTitles, username);
                        await transaction.LogInsertAsync(entityName, "NewAccounts", rows, newAccounts, username);
                        await transaction.LogInsertAsync(entityName, "ProjectDetails", rows, projectDetails, username);
                        await transaction.LogInsertAsync(entityName, "BusinessCase", rows, businessCase, username);
                        await transaction.LogInsertAsync(entityName, "Comments", rows, comments, username);
                        await transaction.LogInsertAsync(entityName, "CreatedDate", rows, createdDate.ToString(), username);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, ex.Message, ex.StackTrace);
                    }

                    transaction.Commit();
                    return rows > 0 ? success(rows) : failed("No rows saved. Error Occured");
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

        public async Task<TResult> UpdateAsync<TResult>(long id, string name, string description, string projectSponsor,
            string executiveSponsor, string productSponsor, long projectTypeId, bool newPricingRules, long volume,
            decimal revenueAtList, bool dealFormEligible, string newTitles, string newAccounts, string projectDetails,
            string businessCase, string comments, DateTime updatedDate, string username, 
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

                    var query = "UPDATE Projects SET " +
                        "[Name] = @Name, " +
                        "[Description] = @Description, " +
                        "[ProjectSponsor] = @ProjectSponsor, " +
                        "[ExecutiveSponsor] = @ExecutiveSponsor, " +
                        "[ProductSponsor] = @ProductSponsor, " +
                        "[ProjectTypeId] = @ProjectTypeId, " +
                        "[NewPricingRules] = @NewPricingRules, " +
                        "[Volume] = @Volume, " +
                        "[RevenueAtList] = @RevenueAtLIst, " +
                        "[DealFormEligible] = @DealFormEligible, " +
                        "[NewTitles] = @NewTitles, " +
                        "[NewAccounts] = @NewAccounts, " +
                        "[ProjectDetails] = @ProjectDetails, " +
                        "[BusinessCase] = @BusinessCase, " +
                        "[Name] = @Name, " +
                        "[Comments] = @Comments, " +
                        "[UpdatedDate] = @UpdatedDate WHERE Id = @Id ";

                    var cmd = new SqlCommand(string.Empty, internalConnection) { CommandText = query };
                    cmd.Parameters.Clear();
                    cmd.Transaction = transaction;

                    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                    cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = name });
                    cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { Value = description });
                    cmd.Parameters.Add(new SqlParameter("@ProjectSponsor", SqlDbType.VarChar) { Value = projectSponsor });
                    cmd.Parameters.Add(new SqlParameter("@ExecutiveSponsor", SqlDbType.VarChar) { Value = executiveSponsor });
                    cmd.Parameters.Add(new SqlParameter("@ProductSponsor", SqlDbType.VarChar) { Value = productSponsor });
                    cmd.Parameters.Add(new SqlParameter("@ProjectTypeId", SqlDbType.Int) { Value = projectTypeId });
                    cmd.Parameters.Add(new SqlParameter("@NewPricingRules", SqlDbType.Bit) { Value = newPricingRules });
                    cmd.Parameters.Add(new SqlParameter("@Volume", SqlDbType.Int) { Value = volume });
                    cmd.Parameters.Add(new SqlParameter("@RevenueAtList", SqlDbType.Decimal) { Value = revenueAtList });
                    cmd.Parameters.Add(new SqlParameter("@DealFormEligible", SqlDbType.Bit) { Value = dealFormEligible });
                    cmd.Parameters.Add(new SqlParameter("@NewTitles", SqlDbType.VarChar) { Value = newTitles });
                    cmd.Parameters.Add(new SqlParameter("@NewAccounts", SqlDbType.VarChar) { Value = newAccounts });
                    cmd.Parameters.Add(new SqlParameter("@ProjectDetails", SqlDbType.VarChar) { Value = projectDetails });
                    cmd.Parameters.Add(new SqlParameter("@BusinessCase", SqlDbType.VarChar) { Value = businessCase });
                    cmd.Parameters.Add(new SqlParameter("@UpdatedDate", SqlDbType.VarChar) { Value = updatedDate });
                    cmd.Parameters.Add(new SqlParameter("@Comments", SqlDbType.VarChar) { Value = comments });

                    var rowsItems = await cmd.ExecuteNonQueryAsync(); var rows = Convert.ToInt64(rowsItems);

                   // TODO: ADD AUDITLOGS

                    transaction.Commit();
                    return rows > 0 ? success() : failed("No rows saved.  Error Occured");
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
                    var query = "Delete From Projects where Id = @Id";

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

        public async Task<TResult> GetAllProjectsAsync<TResult>(ProjectsInfoDelegateAsync<TResult> callback, Func<TResult> done, Func<TResult> notFound)
        {
            using (var internalConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await internalConnection.OpenAsync();

                    var query = "SELECT Id, Name, Description, ProjectSponsor, ExecutiveSponsor, ProductSponsor, ProjectTypeId, NewPricingRules, " +
                        "Volume, RevenueAtList, DealFormEligible, NewTitles, NewAccounts, ProjectDetails, BusinessCase, Comments, CreatedDate FROM Projects";

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
                                reader["ProjectSponsor"].ToString(),
                                reader["ExecutiveSponsor"].ToString(),
                                reader["ProductSponsor"].ToString(),
                                reader["ProjectTypeId"].ToLong(),
                                reader["NewPricingRules"].ToBool(),
                                reader["Volume"].ToLong(),
                                reader["RevenueAtList"].ToDecimal(),
                                reader["DealFormEligible"].ToBool(),
                                reader["NewTitles"].ToString(),
                                reader["NewAccounts"].ToString(),
                                reader["ProjectDetails"].ToString(),
                                reader["BusinessCase"].ToString(),
                                reader["Comments"].ToString(),
                                reader["CreatedDate"].ToDateTime());
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

        public async Task<TResult> FindProjectByIdAsync<TResult>(long id, ProjectInfoDelegateAsync<TResult> callback, Func<TResult> done, Func<TResult> notFound)
        {
            using (var internalConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await internalConnection.OpenAsync();

                    var query = "SELECT Name, Description, ProjectSponsor, ExecutiveSponsor, ProductSponsor, ProjectTypeId, NewPricingRules, " +
                        "Volume, RevenueAtList, DealFormEligible, NewTitles, NewAccounts, ProjectDetails, BusinessCase, Comments, CreatedDate " +
                        "FROM Projects WHERE Id = @Id";

                    var cmd = new SqlCommand(string.Empty, internalConnection) { CommandText = query };
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.BigInt) { Value = id });

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows) return notFound();
                        while (reader.Read())
                        {
                            callback(reader["Name"].ToString(),
                                reader["Description"].ToString(),
                                reader["ProjectSponsor"].ToString(),
                                reader["ExecutiveSponsor"].ToString(),
                                reader["ProductSponsor"].ToString(),
                                reader["ProjectTypeId"].ToLong(),
                                reader["NewPricingRules"].ToBool(),
                                reader["Volume"].ToLong(),
                                reader["RevenueAtList"].ToDecimal(),
                                reader["DealFormEligible"].ToBool(),
                                reader["NewTitles"].ToString(),
                                reader["NewAccounts"].ToString(),
                                reader["ProjectDetails"].ToString(),
                                reader["BusinessCase"].ToString(),
                                reader["Comments"].ToString(),
                                reader["CreatedDate"].ToDateTime());
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
