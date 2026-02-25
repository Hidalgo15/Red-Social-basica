using BookFace.Core.Domain.IRepository;
using BookFace.Infraestructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BookFace.Infraestructure.Persistence.Repository
{
    public class RepositoryService<T> : IRepository<T> where T : class
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<T> _dbSet;
        public RepositoryService(ApplicationContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entidad)
        {
            /*await _context.Set<T>().AddAsync(entidad);*/
            await _dbSet.AddAsync(entidad);
            await _context.SaveChangesAsync();
            return entidad;


        }
        public virtual async Task DeleteAsync(int id)
        {
            /*  var entidad = await GetByIdAsync(id);
              _dbSet.Remove(entidad);
              _context.SaveChanges();*/
            var entidad = await GetByIdAsync(id);
            if (entidad != null) // Verificar si la entidad existe antes de intentar eliminarla
            {
                _dbSet.Remove(entidad);
                await _context.SaveChangesAsync(); // Usar SaveChangesAsync
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            //return await _dbSet.ToListAsync();
            return await _context.Set<T>().ToListAsync();//Deferred execution
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task UpdateAsync(T entidad)
        {
            _dbSet.Attach(entidad);
            _context.Entry(entidad).State = EntityState.Modified;
            // await _dbSet.AddAsync(entidad);
            await _context.SaveChangesAsync();
        }
    }
}
