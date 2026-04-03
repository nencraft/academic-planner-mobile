using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using AcademicPlanner.Models;

namespace AcademicPlanner.Data
{
    public class AcademicPlannerDatabase
    {
        private SQLiteAsyncConnection? _database;
        private bool _initialized;

        private async Task InitAsync()
        {
            if (_initialized && _database is not null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "academicplanner.db3");

            _database = new SQLiteAsyncConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite |
                SQLiteOpenFlags.Create |
                SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<Term>();
            await _database.CreateTableAsync<Course>();

            _initialized = true;
        }

        // Terms
        public async Task<List<Term>> GetTermsAsync()
        {
            await InitAsync();
            return await _database!
                .Table<Term>()
                .OrderBy(t => t.StartDate)
                .ToListAsync();
        }

        public async Task<Term?> GetTermAsync(int id)
        {
            await InitAsync();
            return await _database!
                .Table<Term>()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> SaveTermAsync(Term term)
        {
            await InitAsync();

            if (term.Id != 0)
                return await _database!.UpdateAsync(term);

            return await _database!.InsertAsync(term);
        }

        public async Task<int> DeleteTermAsync(Term term)
        {
            await InitAsync();
            return await _database!.DeleteAsync(term);
        }

        public async Task DeleteTermCascadeAsync(int termId)
        {
            await InitAsync();

            var courses = await GetCoursesByTermAsync(termId);
            foreach (var course in courses)
            {
                await _database!.DeleteAsync(course);
            }

            var term = await GetTermAsync(termId);
            if (term is not null)
            {
                await _database!.DeleteAsync(term);
            }
        }

        // Courses
        public async Task<int> SaveCourseAsync(Course course)
        {
            await InitAsync();

            if (course.Id != 0)
                return await _database!.UpdateAsync(course);

            return await _database!.InsertAsync(course);
        }

        public async Task<List<Course>> GetCoursesByTermAsync(int termId)
        {
            await InitAsync();
            return await _database!
                .Table<Course>()
                .Where(c => c.TermId == termId)
                .OrderBy(c => c.StartDate)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseAsync(int id)
        {
            await InitAsync();
            return await _database!
                .Table<Course>()
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
