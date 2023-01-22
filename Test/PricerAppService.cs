namespace Test;

public partial class PricerAppServiceTest
{
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