using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Context;
using Infrastructure.Entities;

namespace Infrastructure
{
    public class Users
    {
        private readonly ZermosContext _context;

        public Users(ZermosContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all users from the database.
        /// </summary>
        /// <returns>A list of all users.</returns>
        public async Task<List<user>> GetUsersAsync()
        {
            #if RELEASE
            return null;
            #elif DEBUG
            
            try
            {
                return await _context.users.AsNoTracking().ToListAsync();
            }
            catch (DbUpdateException)
            {
                
                return new List<user>();
            }
            #endif
        }

        /// <summary>
        /// Returns a user with the specified uuid.
        /// </summary>
        /// <param name="email">The uuid of the user to return.</param>
        /// <returns>A list of users with the specified uuid.</returns>
        public async Task<user> GetUserAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return new user();
            
            try
            {
                return await _context.users.AsNoTracking().FirstOrDefaultAsync(x => x.email == email.ToLower());
            }
            catch (DbUpdateException)
            {
                // Handle exception (e.g., log it)
                return new user();
            }
        }

        /// <summary>
        /// This method adds a user to the database.
        /// </summary>
        /// <param name="user">The user object to add to the database.</param>
        private async Task AddUserAsync(user user)
        {
            try
            {
                user.email = user.email.ToLower();

                if (await _context.users.AnyAsync(x => x.email == user.email))
                    return;

                await _context.users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                
            }
        }


        /// <summary>
        /// Updates a user with the specified uuid. Note that the uuid cannot be changed. If you leave any field as null, it will not be updated in the database and will keep its current value.
        /// </summary>
        /// <param name="email">The uuid of the user to update.</param>
        /// <param name="user">The user object with the new values.</param>
        public async Task UpdateUserAsync(string email, user user)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return;

                var userToUpdate = await _context.users.FirstOrDefaultAsync(x => x.email == email.ToLower());

                if (userToUpdate == null)
                {
                    await AddUserAsync(user);
                    return;
                }

                var userProperties = typeof(user).GetProperties();

                foreach (var property in userProperties)
                {
                    if (property.PropertyType == typeof(bool))
                        continue;

                    var userValue = property.GetValue(user);

                    if (userValue == null)
                        continue;

                    switch (userValue)
                    {
                        case string value when string.IsNullOrEmpty(value):
                        case DateTime time when time == DateTime.MinValue:
                            property.SetValue(userToUpdate, null);
                            break;
                        default:
                            property.SetValue(userToUpdate, userValue);
                            break;
                    }
                }

                _context.users.Update(userToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                
            }
        }
    }
}