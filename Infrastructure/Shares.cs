using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Context;
using Infrastructure.Entities;

namespace Infrastructure
{
    public class Shares
    {
        private readonly ZermosContext _context;

        public Shares(ZermosContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a share with the specified uuid.
        /// </summary>
        /// <param name="key">The key of the share to return.</param>
        /// <returns>The share with the specified key.</returns>
        public async Task<share> GetShareAsync(string key)
        {
            try
            {
                //get the share using the key, but detach it from the context so that it can be updated later
                return await _context.shares.AsNoTracking().FirstOrDefaultAsync(x => x.key == key);
            }
            catch
            {
                return new share();
            }
        }
        
        /// <summary>
        /// Returns a share with the specified email
        /// </summary>
        /// <param name="email">The email of the share to return.</param>
        /// <returns>The share with the specified key.</returns>
        public async Task<List<share>> GetSharesAsync(string email)
        {
            try
            {
                //get the all shares using the email, but detach it from the context so that it can be updated later
                return await _context.shares.AsNoTracking().Where(x => x.email == email).ToListAsync();
            }
            catch
            {
                return new List<share>();
            }
        }

        /// <summary>
        /// This method adds a share to the database.
        /// </summary>
        /// <param name="share">The share object to add to the database.</param>
        public async Task AddShareAsync(share share)
        {
            share.key ??= Guid.NewGuid().ToString();
            await _context.shares.AddAsync(share);
            await _context.SaveChangesAsync();
        }
        
        /// <summary>
        /// This method deletes a share from the database.
        /// </summary>
        /// <param name="key">The key of the share to delete.</param>
        public async Task DeleteShareAsync(string key)
        {
            var share = await _context.shares.FirstOrDefaultAsync(x => x.key == key.ToLower());
            _context.shares.Remove(share);
            await _context.SaveChangesAsync();
        }
    }
}