using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppName.Web.Repositories
{
    public class AppUser
    {
        public int Id { get; private set; }

        public string Domain { get; private set; }
        
        public string UserName { get; private set; }

        public bool IsActive { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public AppUser(int id, string domain, string userName, bool isActive, string firstName, string lastName)
        {
            Id = id;
            Domain = domain;
            UserName = userName;
            IsActive = isActive;
            FirstName = firstName;
            LastName = lastName;
        }
    }

    public interface IUserRepository
    {
        Task<AppUser> GetUserByUserNameAsync(string domain, string username);
    }

    public class UserRepository : IUserRepository
    {
        private readonly List<AppUser> _users = new List<AppUser>
        {
            new AppUser(1, "developer", "domainuser", true, "Nice", "Person"),
            new AppUser(2, "developer", "diffuser", true, "Naughty", "Person"),
            //new AppUser(3, "yourdomain", "ad.user.name", true, "FirstName", "LastName"),
        };

        public Task<AppUser> GetUserByUserNameAsync(string domain, string username)
        {
            return Task.FromResult(_users.FirstOrDefault(x => x.Domain == domain && x.UserName == username));
        }
    }
}
