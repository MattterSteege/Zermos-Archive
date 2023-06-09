using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Context;
using Infrastructure.Entities;

namespace Infrastructure
{ public class Users
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
            var ranks = await _context.users
                .ToListAsync();

            return await Task.FromResult(ranks);
        }

        /// <summary>
        /// Returns a user with the specified uuid.
        /// </summary>
        /// <param name="email">The uuid of the user to return.</param>
        /// <returns>A list of users with the specified uuid.</returns>
        public async Task<user> GetUserAsync(string email)
        {
            //detach tracking from entity
            
            return await _context.users.FirstOrDefaultAsync(x => x.email == email.ToLower());
        }
        
        /// <summary>
        /// This method adds a user to the database.
        /// </summary>
        /// <param name="user">The user object to add to the database.</param>
        public async Task AddUserAsync(user user)
        {
            user.email = user.email.ToLower();
            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates a user with the specified uuid. Note that the uuid cannot be changed. If you leave any field as null, it will not be updated in the database and will keep its current value.
        /// </summary>
        /// <param name="email">The uuid of the user to update.</param>
        /// <param name="user">The user object with the new values.</param>
        public async Task UpdateUserAsync(string email, user user)
        {
            var userToUpdate = await _context.users.FirstOrDefaultAsync(x => x.email == email.ToLower());

            if (userToUpdate == null)
                return;

            if (user.name != null) userToUpdate.name = user.name;
            if (user.school_id != null) userToUpdate.school_id = user.school_id;
            if (user.school_naam_code != null) userToUpdate.school_naam_code = user.school_naam_code;
            if (user.zermelo_access_token != null) userToUpdate.zermelo_access_token = user.zermelo_access_token;
            if (user.somtoday_access_token != null) userToUpdate.somtoday_access_token = user.somtoday_access_token;
            if (user.somtoday_refresh_token != null) userToUpdate.somtoday_refresh_token = user.somtoday_refresh_token;
            if (user.somtoday_student_id != null) userToUpdate.somtoday_student_id = user.somtoday_student_id;
            if (user.infowijs_access_token != null) userToUpdate.infowijs_access_token = user.infowijs_access_token;
            if (user.infowijs_session_token != null) userToUpdate.infowijs_session_token = user.infowijs_session_token;
            if (user.VerificationToken != null) userToUpdate.VerificationToken = user.VerificationToken;

            await _context.SaveChangesAsync();
        }
    }
}