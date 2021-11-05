using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SastreriaApi.Data;
namespace SastreriaApi.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private SastreriaApiDbContext _context = null;

        public GenericRepository(SastreriaApiDbContext context)
        {
            
            _context = context;
        }

        public void Delete(object id)
        {

       
                T orderToDelete = _context.Set<T>().Find(id);
                _context.Set<T>().Remove(orderToDelete);
     
   
            
        }


        public IQueryable<T> GetAll()
        {

            return _context.Set<T>().AsQueryable();
            //throw new NotImplementedException();
        }

        public Task<T> GetById(object id)
        {
            return _context.Set<T>().FindAsync(id).AsTask();
        }

        public T Insert(T obj)
        {
            var pyvar = _context.Set<T>().Add(obj);
            return pyvar.Entity;
        }

        public void Save()
        {
            _context.SaveChanges();
            
        }

        public void Update(T obj)
        {
            _context.Set<T>().Update(obj);
        }
    }
}
