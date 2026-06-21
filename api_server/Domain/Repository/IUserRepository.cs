using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Domain.Repository;

public interface IUserRepository
{
    // CUD Operations (Create-Read-Delete)
    Task<UserEntity> CreateAsync(UserEntity user, CancellationToken cancellationToken=default);
    Task<UserEntity> UpdateAsync(UserEntity user, CancellationToken cancellationToken=default);
    Task DeleteAsync(UserEntity user, CancellationToken cancellationToken=default);
    
    // Query operations
    Task<UserEntity?> GetById(Guid id, bool isTracked=false, bool eagerLoadRoles=false, CancellationToken cancellationToken=default);
    Task<UserEntity?> GetByEmail(string email, bool isTracked=false, bool eagerLoadRoles=false, CancellationToken cancellationToken=default);
    Task<UserEntity?> GetByUsername(string username, bool isTracked=false, bool eagerLoadRoles=false, CancellationToken cancellationToken=default);
    Task<IEnumerable<UserEntity>> GetAll(int page, int pageSize, CancellationToken cancellationToken=default);
    Task<IEnumerable<UserEntity>> Search(string term, int page, int pageSize, CancellationToken cancellationToken=default);

    Task SaveChanges(CancellationToken cancellationToken=default);
}