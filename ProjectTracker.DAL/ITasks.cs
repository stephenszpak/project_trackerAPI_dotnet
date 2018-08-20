using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.DAL
{
    public delegate T TasksInfoDelegateAsync<T>(long id, string name, string description, bool isComplete);

    public delegate T TaskInfoDelegateAsync<T>(string name, string description, bool isComplete);

    public interface ITasks
    {
        Task<TResult> InsertAsync<TResult>(string name, string description, bool isComplete, string username,
            Func<long, TResult> success,
            Func<string, TResult> failed,
            Func<TResult> unAuthorized);

        Task<TResult> UpdateAsync<TResult>(long id, string name, string description, bool isComplete, string username,
            Func<TResult> success,
            Func<string, TResult> failed,
            Func<TResult> unAuthorized);

        Task<TResult> DeleteAsync<TResult>(long id, string username,
            Func<TResult> success,
            Func<string, TResult> failed);

        Task<TResult> GetAllTasksAsync<TResult>(
            TasksInfoDelegateAsync<TResult> callback,
            Func<TResult> done,
            Func<TResult> notFound);

        Task<TResult> FindTaskByIdAsync<TResult>(long id,
            TaskInfoDelegateAsync<TResult> callback,
            Func<TResult> done,
            Func<TResult> notFound);
    }
}
