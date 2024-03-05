namespace EducationlPlatform.Models.InterfaceHandler
{
    public class RepositoryHandler<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public RepositoryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var entities = await _context.Set<T>().ToListAsync();

            return entities;
        }

        public async Task<T> FindById(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            return entity;
        }

        public async Task<T> Create(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await FindById(id);

            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }

        public async Task<T> Update(int id, T entity)
        {
            T existing = await FindById(id);

            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }

            return entity;
        }

        public async Task<bool> IsExist(int id)
        {
            var entity = await FindById(id);
            return (entity!=null) ? true : false;
        }

        public async Task<int> Count()
        {
            return await _context.Set<T>().CountAsync();
        }


        public IEnumerable<T> Search(Func<T, bool> filter)
        { 
            return _context.Set<T>().Where(filter);
        }
    }
}
