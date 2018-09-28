using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppName.Web.Repositories
{
    public class AppRole
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }
    }

    public interface IRoleRepository
    {
        Task<List<AppRole>> GetRolesForUserAsync(int userId);
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly List<AppRole> _roles = new List<AppRole>
        {
            new AppRole {Id=1, UserId = 1, Name = "admin"},
            new AppRole {Id=2, UserId = 1, Name = "Access.Api"},
            new AppRole {Id=3, UserId = 2, Name = "default"},
        };

        public Task<List<AppRole>> GetRolesForUserAsync(int userId)
        {
            var roles = _roles.Where(r => r.UserId == userId).ToList();

            return Task.FromResult(roles);
        }
    }
}
