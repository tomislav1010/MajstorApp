using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MajstorFinder.DAL.Models;


namespace MajstorFinder.BLL.Interfaces
{

    public interface IAuthService
    {
        AppUser? ValidateLogin(string emailOrUsername, string password);
    }
}