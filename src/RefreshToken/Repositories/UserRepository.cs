using System.Collections.Generic;
using System.Linq;
using Whodo.Autentication.Models;

namespace Whodo.Autentication.Repositories
{
    public static class UserRepository
    {
        public static User Get(string name, string password)
        {
            var users = new List<User>
            {
                new User {Id = 1, Name = "ldcorrea", Role = "developer", Password = "123456"},
                new User {Id = 1, Name = "kessia", Role = "maneger", Password = "123456"}
            };
            return users.FirstOrDefault(x => x.Name == name && x.Password == password);
        }
    }
}
