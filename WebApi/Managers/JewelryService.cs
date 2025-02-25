using WebApi.Models;
using WebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Managers
{
    public class JewelryService : IJewelryService
    {
        private readonly string _filePath = Path.Combine("./", "users.json");
        List<Jewelry> Jewelrys { get; }
        int nextId = 3;
        public JewelryService()
        {
            Jewelrys =
            [
                new Jewelry { Id = 1, Name = "Ring", Price = 12 },
                new Jewelry { Id = 2, Name = "Necklace", Price = 12  }
            ];
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
    }
}