namespace Test
{
    public interface IPriceRepository
    {
        Task InsertAsync(decimal price);
        Task<decimal> FindAsync();
        Task<decimal> FindAsync(bool only);
        decimal Find();
        string Find2();
        int Get<T>();
    }
}
