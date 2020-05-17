using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopDataAccess.SQL.Repositories
{
    public class UnitOfWorkRepositories : IDisposable
    {
        #region Properties

        private DataContext _context = new DataContext();
        private Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        #endregion

        public TRepo Repository<TRepo>()
        {
            if (repositories.Keys.Contains(typeof(TRepo)) == true)
                return (TRepo)repositories[typeof(TRepo)];

            TRepo repo = (TRepo)Activator.CreateInstance(
                typeof(TRepo),
                new object[] { _context });

            repositories.Add(typeof(TRepo), repo);
            return repo;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
