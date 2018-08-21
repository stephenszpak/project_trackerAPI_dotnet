using ProjectTracker.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    public class Projects
    {
        private readonly Context _context;
        private readonly IDataContext _dataContext;

        public delegate T ProjectsInfoDelegate<T>(long id, string name, string description, string projectSponsor, string executiveSponsor,
            string productSponsor, long projectTypeId, bool newPricingRules, long volume, decimal revenueAtList,
            bool dealFormEligible, string newTitles, string newAccounts, string projectDetails, string businessCase,
            string comments, DateTime createdDate);

        public delegate T ProjectInfoDelegate<T>(string name, string description, string projectSponsor, string executiveSponsor,
            string productSponsor, long projectTypeId, bool newPricingRules, long volume, decimal revenueAtList,
            bool dealFormEligible, string newTitles, string newAccounts, string projectDetails, string businessCase,
            string comments, DateTime createdDate);

        public Projects(Context context, IDataContext dataContext)
        {
            _context = context;
            _dataContext = dataContext;
        }

        public async Task<TResult> InsertAsync<TResult>(string name, string description, string projectSponsor, string executiveSponsor,
            string productSponsor, long projectTypeId, bool newPricingRules, long volume, decimal revenueAtList,
            bool dealFormEligible, string newTitles, string newAccounts, string projectDetails, string businessCase,
            string comments, DateTime createdDate, string username,
            Func<long, TResult> success,
            Func<string, TResult> failed,
            Func<TResult> unAuthorized)
        {
            return await _dataContext.Projects.InsertAsync(name, description, projectSponsor, executiveSponsor, productSponsor, projectTypeId, 
                newPricingRules, volume, revenueAtList, dealFormEligible, newTitles, newAccounts, projectDetails, businessCase, comments, createdDate, username,
                id => { return success(id); },
                s => { return failed(s); },
                () => { return unAuthorized(); }
            );
        }

        public async Task<TResult> DeleteAsync<TResult>(long id, string username,
            Func<TResult> success,
            Func<string, TResult> failed)
        {
            return await _dataContext.Projects.DeleteAsync(id, username,
                () => { return success(); },
                s => { return failed(s); });
        }
    }
}
