using System.Linq.Expressions;
namespace ServiPuntosUy.Models.DAO;
public interface IGenericRepository<T> where T : class
{
    // Operaciones básicas CRUD
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);

    // Operaciones de guardado
    Task SaveChangesAsync();

    // Operaciones de consulta
    IQueryable<T> GetQueryable();
    IQueryable<T> Get(List<Expression<Func<T, bool>>>? conditions = null);
}
