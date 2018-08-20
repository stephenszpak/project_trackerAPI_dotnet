using ProjectTracker.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    public class Context
    {
        protected Context() { }

        private IDataContext dataContext;
        private readonly Func<IDataContext> dataContextCreateFunc;

        public Context(Func<IDataContext> dataContextCreateFunc)
        {
            if (dataContextCreateFunc != null)
            {
                this.dataContextCreateFunc = dataContextCreateFunc;
            }
        }

        public IDataContext DataContext => dataContext ?? (dataContext = dataContextCreateFunc.Invoke());

        private Authorizations authorizations = default(Authorizations);
        public Authorizations Authorizations => authorizations ?? (authorizations = new Authorizations(this, DataContext));

        private Projects projects = default(Projects);
        public Projects Projects => projects ?? (projects = new Projects(this, DataContext));

        private Tasks tasks = default(Tasks);
        public Tasks Tasks => tasks ?? (tasks = new Tasks(this, DataContext));

    }
}
