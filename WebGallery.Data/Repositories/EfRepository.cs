using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

namespace WebGallery.Data.Repositories;

public interface IRepository<T> : IRepositoryBase<T> where T : class
{
}

public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    public EfRepository(DatabaseContext databaseContext) : base(databaseContext)
    {
    }

    public EfRepository(DatabaseContext databaseContext, ISpecificationEvaluator specificationEvaluator)
        : base(databaseContext, specificationEvaluator)
    {
    }
}
