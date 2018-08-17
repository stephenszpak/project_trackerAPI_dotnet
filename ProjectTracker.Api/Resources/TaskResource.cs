using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTracker.Api.Resources
{
    public class TaskResource
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
    }
}