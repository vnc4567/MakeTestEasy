namespace Test
{
    public interface IPricerAppService
    {
        int Get<T>();
        Task SaveFuturPrice();
    }

    public class DontMock : IDontMock
    {
        public string GetTest() => "test";
    }

    public class DontMock2 : IDontMock2
    {
        public string GetTest2() => "test2";
    }

    public class PricerAppService : IPricerAppService
    {
        private readonly IPriceRepository _priceRepository;
        private readonly IDontMock _dontMock;
        private readonly IDontMock2 _dontMock2;

        public PricerAppService(IPriceRepository priceRepository, IDontMock dontMock, IDontMock2 dontMock2)
        {
            _priceRepository = priceRepository;
            _dontMock = dontMock;
            _dontMock2 = dontMock2;
        }

        public async Task SaveFuturPrice()
        {
            decimal value = await _priceRepository.FindAsync();
            decimal value2 = await _priceRepository.FindAsync(true);
            await _priceRepository.InsertAsync(value * 2);
        }

        public string GetTest() => _dontMock.GetTest();
        public string GetTest2() => _dontMock2.GetTest2();

        public int Get<T>()
        {
            return _priceRepository.Get<T>();
        }
    }
}
