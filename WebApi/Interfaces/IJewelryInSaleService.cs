using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IJewelryInSaleService : IGeneralService<JewelryInSale>
    {
        public List<JewelryInSale> GetAll(int id);
    }
}
