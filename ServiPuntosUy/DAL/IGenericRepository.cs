namespace ServiPuntosUy.Models.DAO;

public interface IGenericRepository<T> where T : class
{
    // Operaciones básicas CRUD
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(params object[] keyValues);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task DeleteAsync(params object[] keyValues);

    // Operaciones de guardado
    Task SaveChangesAsync();

    // Operaciones de consulta
    IQueryable<T> GetQueryable();
}
