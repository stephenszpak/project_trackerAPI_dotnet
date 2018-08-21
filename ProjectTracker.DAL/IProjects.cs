using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.DAL
{

    public delegate T ProjectsInfoDelegateAsync<T>(long id, string name, string description, string projectSponsor, string executiveSponsor,
        string productSponsor, long projectTypeId, bool newPricingRules, long volume, decimal revenueAtList,
        bool dealFormEligible, string newTitles, string newAccounts, string projectDetails, string businessCase,
        string comments, DateTime createdDate);

    public delegate T ProjectInfoDelegateAsync<T>(string name, string description, string projectSponsor, string executiveSponsor,
        string productSponsor, long projectTypeId, bool newPricingRules, long volume, decimal revenueAtList,
        bool dealFormEligible, string newTitles, string newAccounts, string projectDetails, string businessCase,
        string comments, DateTime createdDate);

    public interface IProjects
    {
        Task<TResult> InsertAsync<TResult>(string name, string description, string projectSponsor, string executiveSponsor,
            string productSponsor, long projectTypeId, bool newPricingRules, long volume, decimal revenueAtList,
            bool dealFormEligible, string newTitles, string newAccounts, string projectDetails, string businessCase,
            string comments, DateTime createdDate, string username,
            Func<long, TResult> success,
            Func<string, TResult> failed,
            Func<TResult> unAuthorized);

        Task<TResult> UpdateAsync<TResult>(long id, string name, string description, string projectSponsor, string executiveSponsor,
            string productSponsor, long projectTypeId, bool newPricingRules, long volume, decimal revenueAtList,
            bool dealFormEligible, string newTitles, string newAccounts, string projectDetails, string businessCase,
            string comments, DateTime updatedDate, string username,
            Func<TResult> success,
            Func<string, TResult> failed,
            Func<TResult> unAuthorized);

        Task<TResult> DeleteAsync<TResult>(long id, string username,
            Func<TResult> success,
            Func<string, TResult> failed);

        Task<TResult> GetAllProjectsAsync<TResult>(
            ProjectsInfoDelegateAsync<TResult> callback,
            Func<TResult> success,
            Func<TResult> notFound);

        Task<TResult> FindProjectByIdAsync<TResult>(long id,
            ProjectInfoDelegateAsync<TResult> callback,
            Func<TResult> done,
            Func<TResult> notFound);
    }

}
