using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;
using MajstorFinder.DAL.Models;


namespace MajstorFinder.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly MajstoriDbContext _db;
        public AuthService(MajstoriDbContext db) => _db = db;

        public AppUser? ValidateLogin(string emailOrUsername, string password)
        {
            var user = _db.AppUsers
                .FirstOrDefault(u => u.Email == emailOrUsername || u.Username == emailOrUsername);

            if (user == null) return null;

            // koristi istu logiku kao u WebAPI (PasswordHasher helper)
            // pretpostavka: imaš PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt, user.Iterations)
            var ok = PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt, user.Iterations);
            return ok ? user : null;
        }
    }
}