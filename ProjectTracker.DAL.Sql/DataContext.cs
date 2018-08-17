﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.DAL.Sql
{
    public class DataContext : IDataContext
    {
        private readonly string connectionString = string.Empty;

        public DataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private Tasks tasks = default(Tasks);
        public ITasks Tasks
        {
            get
            {
                if (default(Tasks) == tasks)
                    tasks = new Tasks(connectionString);
                return tasks;
            }
        }
    }
}
