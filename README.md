# MakeTestEasy

## Help to make test

This library helps you to make test easily.

### Main feature

Your test class should derive from BaseTest<T>. T is the class under test
All dependancies of your class under test is mocked. You can pass a real implementation of a dependencie instead of mock.
You can retreive all mocked object with method Get<Tdependencie> and you can do all Moq stuff from it. You have the possibility to setup a method for your mock without know the parameters of this method. 

  Let's take an example :

  We have a class PricerAppService that have 2 dependencies
  
  ```
   public class PricerAppService : IPricerAppService
   {
        private readonly IPriceRepository _priceRepository;
        private readonly IDontMock _dontMock;

        public PricerAppService(IPriceRepository priceRepository, IDontMock dontMock)
        {
            _priceRepository = priceRepository;
            _dontMock = dontMock;
        }
  
          public async Task SaveFuturPriceAsync()
          {
              decimal value = await _priceRepository.FindAsync();
              
              decimal rate = _dontMock.GetRate();
  
              await _priceRepository.InsertAsync(value * rate);
          }
  
          public int Get<T>()
          {
              return _priceRepository.Get<T>();
          }
  }
  
  public class DontMock : IDontMock 
  {
      public decimal GetRate() => 2;
  }
  ```
  
  Now we want to test our method SaveFuturPriceAsync. This is the test class :

  ```
   public class PricerAppServiceTest : BaseTest<PricerAppService>
   {
        public PricerAppServiceTest()
        {
            IDontMock dontMock = new DontMock();
            Create().ExcludeDependenciesToBeMock(dontMock).Build();
        }

        [Fact]
        public async Task SaveFuturePrice()
        {
            Get<IPriceRepository>().SetupWithoutParamAsync("FindAsync", 5m);

            await sut.SaveFuturPrice();

            Get<IPriceRepository>().Verify(x => x.InsertAsync(10));

        }
  }
  ```
  The line Create().ExcludeDependenciesToBeMock(dontMock).Build() is needed only if we don't want to mock a specific dependancie.
  
The line Get<IPriceRepository>().SetupWithoutParamAsync("FindAsync", 5m) retreive the mock for IPriceRepository and indicate that mock should return 5 for method FindAsync no matters parametters pass on this method
  
  
