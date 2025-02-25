using WebApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Interfaces
{
    public interface IJewelryService
    {
        List<Jewelry> GetAll();

        Jewelry Get(int id);

        void Add(Jewelry pizza);

        void Delete(int id);

        void Update(Jewelry jewelry);

        int Count { get; }
    }
}