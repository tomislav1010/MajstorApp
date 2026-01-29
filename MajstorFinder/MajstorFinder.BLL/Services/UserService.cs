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
