using MajstorFinder.BLL.DTOs;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;
using MajstorFinder.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajstorFinder.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly MajstoriDbContext _db;
        public UserService(MajstoriDbContext db) {
            _db = db;
        }

        public async Task<bool> CreateAsync(CreateUserDto dto)
        {
            if (await _db.AppUsers.AnyAsync(u =>
                u.Email == dto.Email || u.Username == dto.Username))
                return false;

            PasswordHasher.Create(
                dto.Password,
                out byte[] hash,
                out byte[] salt,
                out int iterations
            );

            var user = new AppUser
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hash,        // byte[]
                PasswordSalt = salt,        // byte[]
                Iterations = iterations,
                CreatedAt = DateTime.Now,
                IsAdmin = dto.IsAdmin
            };

            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public  Task<AppUser?> GetByIdAsync(int id)
        =>_db.AppUsers.FirstOrDefaultAsync(x=>x.Id==id);
        

        public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
        {
           var u= await _db.AppUsers.FirstOrDefaultAsync(x => x.Id==id);
            if (u != null) return false;

            var exists = await _db.AppUsers.AnyAsync(x => x.Id!=id && (x.Email==dto.Email || x.Username==dto.Username));
            if (exists) return false;

            u.Username = dto.Username.Trim();
            u.Email = dto.Email.Trim();
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
