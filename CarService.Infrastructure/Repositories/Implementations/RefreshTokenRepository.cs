using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarService.Infrastructure.Data;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure.Repositories.Implementations
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.refreshTokens.AddAsync(token);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.refreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}