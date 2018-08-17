using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.DAL.Sql
{
    public enum UpdateType
    {
        Create,
        Update,
        Delete
    }

    public static partial class Extensions
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        //insert
        public static async Task<bool> LogInsertAsync(this SqlTransaction xaction, string objectType, string property, long objectId, string newValue, string user)
        {
            user = user ?? Environment.UserName;

            System.Diagnostics.Debug.WriteLine($"objectType: {objectType} property: {property} objectId: {objectId} newValue: {newValue} user: {user}");

            var date = DateTime.Now;
            var changeGroup = $"{date:yyyyMMdd.hhmmss}.{objectId}.{objectType}.{user}";

            return await LogAuditAsync(xaction, user, objectType, objectId, property, UpdateType.Create, date,
                null, newValue, changeGroup);

        }

        //update
        public static async Task<bool> LogUpdateAsync(this SqlTransaction xaction, string objectType, string property, long objectId, string oldValue, string newValue, string user)
        {
            if ((oldValue ?? string.Empty) == (newValue ?? string.Empty))
            {
                return true;
            }

            user = user ?? Environment.UserName;

            var date = DateTime.Now;
            var changeGroup = $"{date:yyyyMMdd.hhmmss}.{objectId}.{objectType}.{user}";

            return await LogAuditAsync(xaction, user, objectType, objectId, property, UpdateType.Update, date,
                oldValue, newValue, changeGroup);

        }

        //delete
        public static async Task<bool> LogDeleteAsync(this SqlTransaction xaction, string objectType, long objectId, string user)
        {
            user = user ?? Environment.UserName;

            var date = DateTime.Now;
            var changeGroup = $"{date:yyyyMMdd.hhmmss}.{objectId}.{objectType}.{user}";

            return await LogAuditAsync(xaction, user, objectType, objectId, null, UpdateType.Delete, date,
                null, null, changeGroup);
        }

        private static async Task<bool> LogAuditAsync(SqlTransaction transaction, string user, string objectType, long objectId, string property, UpdateType updateType, DateTime dateChanged, string oldValue,
            string newValue, string changeGroup)
        {
            try
            {
                var query = "INSERT INTO AuditLogs([User], [DateChanged], [ObjectType], [ObjectId], [Property], [OldValue], [NewValue], [UpdateType], [ChangeGroup])";
                query = query + " VALUES ";
                query = query + "(@User, @DateChanged, @ObjectType, @ObjectId, @Property, @OldValue, @NewValue, @UpdateType, @ChangeGroup); SELECT SCOPE_IDENTITY()";


                var cmd = new SqlCommand(string.Empty, transaction.Connection) { CommandText = query };
                cmd.Parameters.Clear();
                cmd.Transaction = transaction;

                cmd.Parameters.Add(new SqlParameter("@User", SqlDbType.VarChar) { Value = user });
                cmd.Parameters.Add(new SqlParameter("@DateChanged", SqlDbType.DateTime) { Value = dateChanged });
                cmd.Parameters.Add(new SqlParameter("@ObjectType", SqlDbType.VarChar) { Value = objectType });
                cmd.Parameters.Add(new SqlParameter("@ObjectId", SqlDbType.BigInt) { Value = objectId });
                cmd.Parameters.Add(new SqlParameter("@Property", SqlDbType.VarChar) { Value = property ?? "" });
                cmd.Parameters.Add(new SqlParameter("@OldValue", SqlDbType.VarChar) { Value = oldValue ?? "" });
                cmd.Parameters.Add(new SqlParameter("@NewValue", SqlDbType.VarChar) { Value = newValue ?? "" });
                cmd.Parameters.Add(new SqlParameter("@UpdateType", SqlDbType.VarChar) { Value = updateType });
                cmd.Parameters.Add(new SqlParameter("@ChangeGroup", SqlDbType.VarChar) { Value = changeGroup });

                var rowsItems = await cmd.ExecuteScalarAsync(); var rows = Convert.ToInt64(rowsItems);

                return rows > 0 ? true : false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, sqlEx.Message, sqlEx.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
