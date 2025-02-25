using System.Security.Claims;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        User Get(int userId);
        void Add(User user);
        void Update(User user);
        void Delete(int id);
        int Count { get; }
    }
}
