using ProjectTracker.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    public class Tasks
    {
        private readonly Context _context;
        private readonly IDataContext _dataContext;

        public delegate T TasksInfoDelegate<T>(long id, string name, string description, bool isComplete);
        public delegate T TaskInfoDelegate<T>(string name, string description, bool isComplete);

        public Tasks(Context context, IDataContext dataContext)
        {
            _context = context;
            _dataContext = dataContext;
        }

        public async Task<TResult> InsertAsync<TResult>(string name, string description, bool isComplete, string username,
            Func<long, TResult> success,
            Func<string, TResult> failed,
            Func<TResult> unAuthorized)
        {
            return await _dataContext.Tasks.InsertAsync(name, description, isComplete, username,
                id => { return success(id); },
                s => { return failed(s); },
                () => { return unAuthorized(); }
            );
        }

        public async Task<TResult> UpdateAsync<TResult>(long id, string name, string description, bool isComplete, string username,
            Func<TResult> success,
            Func<string, TResult> failed,
            Func<TResult> unAuthorized)
        {
            return await _dataContext.Tasks.UpdateAsync(id, name, description, isComplete, username,
                () => { return success(); },
                s => { return failed(s); },
                () => { return unAuthorized(); }
            );
        }

        public async Task<TResult> DeleteAsync<TResult>(long id, string username,
            Func<TResult> success,
            Func<string, TResult> failed)
        {
            return await _dataContext.Tasks.DeleteAsync(id, username,
                () => { return success(); },
                s => { return failed(s); });
        }

        public async Task<TResult> GetAllTasksAsync<TResult>(
            TasksInfoDelegate<TResult> success,
            Func<TResult> done,
            Func<TResult> notFound)
        {
            return await _dataContext.Tasks.GetAllTasksAsync(
                (id, name, description, isComplete) =>
                {
                    return success(id, name, description, isComplete);
                },
                () => { return done(); },
                () => { return notFound(); }

            );
        }

            public async Task<TResult> FindTasksByIdAsync<TResult>(long id,
            TaskInfoDelegate<TResult> success,
            Func<TResult> done,
            Func<TResult> notFound)
        {
            return await _dataContext.Tasks.FindTaskByIdAsync(id,
                (name, description, isComplete) =>
                {
                    return success(name, description, isComplete);
                },
                () => { return done(); },
                () => { return notFound(); }
            );
        }
    }
}
