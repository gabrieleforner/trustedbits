using System.Collections;
using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Infrastructure.EFCore;

namespace Trustedbits.ApiServer.Domain.Repository.impl;

public class UserRepositoryImpl : IUserRepository
{
    private readonly DbSet<UserEntity> _dbSet;
    private readonly ServerDbContext _context;

    public UserRepositoryImpl(DbSet<UserEntity> dbSet, ServerDbContext context)
    {
        _dbSet = dbSet;
        _context = context;
    }

    public async Task<UserEntity> CreateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<UserEntity> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var entityState = _context.Entry(user).State;
        switch (entityState)
        {
            case EntityState.Modified:
            {
                // Handle tracked entities update
                await _context.SaveChangesAsync(cancellationToken);
                return user;
            }
            case EntityState.Detached:
            {
                // Handle untracked entities update
                var result = _dbSet.Update(user);
                await _context.SaveChangesAsync(cancellationToken);
                return result.Entity;
            }
            // Theoretically never hit, should always fall in one of the two cases
            default:
                return user;
        }
    }

    public async Task DeleteAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserEntity?> GetById(Guid id, bool isTracked = false, bool eagerLoadRoles = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = isTracked ? query.AsTracking() : query.AsNoTracking();      // Handle entity tracking

        if(eagerLoadRoles)
            query = query.Include(u => u.AssociatedRoles);
    
        return await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<UserEntity?> GetByEmail(string email, bool isTracked = false, bool eagerLoadRoles = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = isTracked ? query.AsTracking() : query.AsNoTracking();      // Handle entity tracking
        if(eagerLoadRoles)
            query = query.Include(u => u.AssociatedRoles);
        return await query.FirstOrDefaultAsync(x => x.NormalizedEmail.Equals(email.ToLower()), cancellationToken);
    }

    public async Task<UserEntity?> GetByUsername(string username, bool isTracked = false, bool eagerLoadRoles = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = isTracked ? query.AsTracking() : query.AsNoTracking();      // Handle entity tracking
        if(eagerLoadRoles)
            query = query.Include(u => u.AssociatedRoles);
        return await query.FirstOrDefaultAsync(x => x.NormalizedUsername.Equals(username.ToLower()), cancellationToken);
    }

    public async Task<IEnumerable<UserEntity>> GetAll(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var entries = await _dbSet
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return entries;
    }

    public async Task<IEnumerable<UserEntity>> Search(string term, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var matching = await _dbSet
            .AsNoTracking()
            .Where(u => 
                u.NormalizedEmail.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                u.NormalizedUsername.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return matching;
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}