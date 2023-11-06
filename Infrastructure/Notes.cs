using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Context;
using Infrastructure.Entities;

namespace Infrastructure
{
    public class Notes
    {
        private readonly ZermosContext _context;

        public Notes(ZermosContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all notes from the database.
        /// </summary>
        /// <returns>A list of all notes.</returns>
        public async Task<List<note>> GetNotesAsync()
        {
            #if RELEASE
            return null;
            #elif DEBUG
            
            try
            {
                return await _context.notes.AsNoTracking().ToListAsync();
            }
            catch
            {
                return new List<note>();
            }
            #endif
        }

        /// <summary>
        /// Returns a note with the specified uuid.
        /// </summary>
        /// <param name="id">The uuid of the note to return.</param>
        /// <returns>A list of notes with the specified uuid.</returns>
        public async Task<note> GetNoteAsync(string id)
        {
            try
            {
                //get the note using the id, but detach it from the context so that it can be updated later
                return await _context.notes.AsNoTracking().FirstOrDefaultAsync(x => x.id == id);
            }
            catch
            {
                return new note();
            }
        }

        /// <summary>
        /// This method adds a note to the database.
        /// </summary>
        /// <param name="note">The note object to add to the database.</param>
        /// 
        public async Task<string> AddNoteAsync(note note)
        {
            try
            {
                note.id = Guid.NewGuid().ToString();

                if (await _context.notes.AnyAsync(x => x.id == note.id))
                    return null;

                await _context.notes.AddAsync(note);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // ignored
            }
            
            return note.id;
        }


        /// <summary>
        /// Updates a note with the specified uuid. Note that the uuid cannot be changed. If you leave any field as null, it will not be updated in the database and will keep its current value.
        /// </summary>
        /// <param name="id">The uuid of the note to update.</param>
        /// <param name="note">The note object with the new values.</param>
        public async Task UpdateNoteAsync(string id, note note)
        {
            if (string.IsNullOrEmpty(id))
                return;
            
            var noteToUpdate = await _context.notes.FirstOrDefaultAsync(x => x.id == id);

            if (noteToUpdate == null)
                await AddNoteAsync(note);

            var noteProperties = typeof(note).GetProperties();

            foreach (var property in noteProperties)
            {
                if (property.PropertyType == typeof(bool))
                    continue;
                
                var noteValue = property.GetValue(note);

                if (noteValue == null)
                    continue;
                
                switch (noteValue)
                {
                    case string value when string.IsNullOrEmpty(value):
                    case DateTime time when time == DateTime.MinValue:
                        property.SetValue(noteToUpdate, null);
                        break;
                    default:
                        property.SetValue(noteToUpdate, noteValue);
                        break;
                }
            }

            if (noteToUpdate != null) _context.notes.Update(noteToUpdate);
            await _context.SaveChangesAsync();
        }
    }
}