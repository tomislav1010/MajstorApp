using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MajstorFinder.BLL.DTOs;
using MajstorFinder.DAL.Models;


namespace MajstorFinder.BLL.Interfaces
{
    public interface IUserService
    {
        Task<AppUser?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateUserDto dto);

        Task<bool> CreateAsync(CreateUserDto dto);

    }
}
