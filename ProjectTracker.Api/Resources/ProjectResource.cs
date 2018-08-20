using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTracker.Api.Resources
{
    public class ProjectResource
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectSponsor { get; set; }
        public string ExecutiveSponsor { get; set; }
        public string ProductSponsor { get; set; }
        public long ProjectTypeId { get; set; }
        public bool NewPricingRules { get; set; }
        public long Volume { get; set; }
        public decimal RevenueAtList { get; set; }
        public bool DealFormEligible { get; set; }
        public string NewTitles { get; set; }
        public string NewAccounts { get; set; }
        public string ProjectDetails { get; set; }
        public string BusinessCase { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}