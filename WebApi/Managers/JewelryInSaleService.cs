using WebApi.Models;
using WebApi.Interfaces;
using Newtonsoft.Json;
using System.Text.Json;

namespace WebApi.Managers
{
    public class JewelryInSaleService : IJewelryInSaleService
    {
        private readonly string _filePath = "../wwwroot/data/JewelryInSale.json";
        List<JewelryInSale> Jewelrys { get; }
        int nextId = 3;
        public JewelryInSaleService()
        {
            var json = File.ReadAllText(_filePath);
            Jewelrys = JsonConvert.DeserializeObject<List<JewelryInSale>>(json);
            Console.WriteLine("fetch all data from server. <JewelryInSale>");
        }

        public List<JewelryInSale> GetAll() => Jewelrys;

        public JewelryInSale Get(int id) => Jewelrys.FirstOrDefault(p => p.UserId == id) ?? new JewelryInSale();

        public List<JewelryInSale> GetAll(int id) => Jewelrys.FindAll(p => p.UserId == id) ?? [];

        public void Add(JewelryInSale Jewelry)
        {
            Jewelrys.Add(Jewelry);
            SaveJewelrys(Jewelrys);
        }

        public void Delete(int id)
        {
            List<JewelryInSale> Jewelry = GetAll(id);
            foreach (var item in Jewelry)
            {
                
            }
            if (Jewelry is null)
                return;
            Jewelrys.Remove(new JewelryInSale());
            SaveJewelrys(Jewelrys);
        }

        public void Update(JewelryInSale Jewelry)
        {
            var index = Jewelrys.FindIndex(p => p.UserId == Jewelry.UserId);
            if (index == -1)
                return;
            Jewelrys[index] = Jewelry;
        }

        public int Count { get => Jewelrys.Count; }

        public void SaveJewelrys(List<JewelryInSale> users)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}