namespace WebApi.Interfaces
{
    public interface IGeneralService<T>
    {
        List<T> GetAll();

        T Get(int id);

        void Add(T pizza);

        void Delete(int id);

        void Update(T jewelry);

        int Count { get; }

        void SaveJewelrys(List<T> items);

    }
}