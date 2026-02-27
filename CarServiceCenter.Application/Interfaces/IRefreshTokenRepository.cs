using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarServiceCenter.Domain.Entities;

namespace CarServiceCenter.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task SaveChangesAsync();
    }
}