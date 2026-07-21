using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Domain.Repository;

namespace Trustedbits.ApiServer.Infrastructure.EntityFramework.RepositoryImpl;

public class ScopeRepositoryImpl : IScopeRepository
{
    private readonly ServerDbContext _dbContext;
    private readonly DbSet<Scope> _scopes;

    public ScopeRepositoryImpl(ServerDbContext dbContext, DbSet<Scope> scopes)
    {
        _dbContext = dbContext;
        _scopes = scopes;
    }

    public async Task CreateScopeAsync(Scope scope, CancellationToken cancellationToken = default)
    {
        try
        {
            await _scopes.AddAsync(scope, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintException e)
        {
            throw new RepositoryException(e.Message, RepositoryExceptionType.UniqueValueViolation);
        }
        catch (CannotInsertNullException e)
        {
            throw new RepositoryException(e.Message, RepositoryExceptionType.NonNullableValueException);
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, RepositoryExceptionType.ServerException);
        }
    }

    public async Task<Scope?> GetByIdAsync(Guid scopeId, bool tracking = false, bool loadRoles = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildScopeQuery(tracking, loadRoles);
        return await query.FirstOrDefaultAsync(s => s.Id == scopeId, cancellationToken);
    }

    public async Task<Scope?> GetByNameAsync(string scopeName, bool tracking = false, bool loadRoles = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildScopeQuery(tracking, loadRoles);
        return await query.FirstOrDefaultAsync(s => s.Name == scopeName, cancellationToken);
    }

    public async Task<Scope?> GetByValueAsync(string scopeValue, bool tracking = false, bool loadRoles = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildScopeQuery(tracking, loadRoles);
        return await query.FirstOrDefaultAsync(s => s.Value == scopeValue, cancellationToken);
    }

    public async Task<IEnumerable<Scope>> GetAll(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _scopes
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Scope>> SearchAsync(string queryTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _scopes
            .AsNoTracking()
            .Where(s => s.Name.Contains(queryTerm) || s.Value.Contains(queryTerm))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateScopeAsync(Scope target, Scope values, CancellationToken cancellationToken = default)
    {
        try
        {
            _dbContext.Entry(target).CurrentValues.SetValues(values);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintException e)
        {
            throw new RepositoryException(e.Message, RepositoryExceptionType.UniqueValueViolation);
        }
        catch (CannotInsertNullException e)
        {
            throw new RepositoryException(e.Message, RepositoryExceptionType.NonNullableValueException);
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, RepositoryExceptionType.ServerException);
        }
    }

    public async Task DeleteScopeAsync(Scope scope, CancellationToken cancellationToken = default)
    {
        try
        {
            _scopes.Remove(scope);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e)
        {
            throw new RepositoryException(e.Message, RepositoryExceptionType.ServerException);
        }
    }

    private IQueryable<Scope> BuildScopeQuery(bool tracking, bool loadRoles)
    {
        var query = _scopes.AsQueryable();

        return tracking ? query.AsTracking() : query.AsNoTracking();
    }
}