using System.Text.Json;
using Newtonsoft.Json;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Managers
{
    public class UserService : IUserService
    {
        private readonly string _filePath = "./wwwroot/data/users.json";
        List<User> Users { get; }
        int nextId = 3;
        public UserService()
        {
            var json = File.ReadAllText(_filePath);
            Users = JsonConvert.DeserializeObject<List<User>>(json);
            foreach (User User1 in Users) { Console.WriteLine($"{User1.Id},{User1.UserName},{User1.Password},{User1.Type},{User1.Basket}"); }
            Console.WriteLine("fetch all data from server.");
        }

        public List<User> GetAll() => Users;

        public User Get(int id) => Users.FirstOrDefault(p => p.Id == id) ?? new User();

        public void Add(User user)
        {
            user.Id = nextId++;
            Users?.Add(user);
        }

        public void Delete(int id)
        {
            var user = Get(id);
            if (user is null)
                return;
            Users?.Remove(user);
        }

        public void Update(User User)
        {
            var index = Users!.FindIndex(p => p.Id == User.Id);
            if (index == -1)
                return;
            Users[index] = User;
        }

        public int Count { get => Users.Count; }

        public void SaveJewelrys(List<User> users)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}