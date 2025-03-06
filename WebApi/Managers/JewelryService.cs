using WebApi.Models;
using WebApi.Interfaces;
using Newtonsoft.Json;
using System.Text.Json;

namespace WebApi.Managers
{
    public class JewelryService : IJewelryService
    {
        private readonly string _filePath = "../wwwroot/data/users.json";
        List<Jewelry> Jewelrys { get; }
        int nextId = 3;
        public JewelryService()
        {
            var json = File.ReadAllText(_filePath);
            Jewelrys = JsonConvert.DeserializeObject<List<Jewelry>>(json);
            Console.WriteLine("fetch all data from server.");
        }

        public List<Jewelry> GetAll() => Jewelrys;

        public Jewelry Get(int id) => Jewelrys.FirstOrDefault(p => p.Id == id) ?? new Jewelry();

        public void Add(Jewelry Jewelry)
        {
            Jewelry.Id = nextId++;
            Jewelrys.Add(Jewelry);
        }

        public void Delete(int id)
        {
            var Jewelry = Get(id);
            if (Jewelry is null)
                return;
            Jewelrys.Remove(Jewelry);
        }

        public void Update(Jewelry Jewelry)
        {
            var index = Jewelrys.FindIndex(p => p.Id == Jewelry.Id);
            if (index == -1)
                return;
            Jewelrys[index] = Jewelry;
        }

        public int Count { get => Jewelrys.Count; }

        public void SaveJewelrys(List<Jewelry> users)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}