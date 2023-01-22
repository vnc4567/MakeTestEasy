using FluentAssertions;
using MakeTestEasy;
using Xunit;

namespace Test
{
    public partial class PricerAppServiceTest : BaseTest<PricerAppServiceTest.PricerAppService>
    {
        public PricerAppServiceTest()
        {
            IDontMock dontMock = new DontMock();
            IDontMock2 dontMock2 = new DontMock2();
            Create().ExcludeDependenciesToBeMock(dontMock2).ExcludeDependenciesToBeMock(dontMock).Build();
        }

        [Fact]
        public async Task SaveFuturePrice()
        {
            Get<IPriceRepository>().SetupWithoutParamAsync("FindAsync", 5m);

            await Sut.SaveFuturPrice();

            Get<IPriceRepository>().Verify(x => x.InsertAsync(10));
        }

        [Fact]
        public void ShouldNotMockSpecificClass()
        {
            string result = Sut.GetTest();

            result.Should().Be("test");

            result = Sut.GetTest2();

            result.Should().Be("test2");
        }

        [Fact]
        public void ShouldWorkWithGenericMethod()
        {
            Get<IPriceRepository>().SetupWithoutParam("Get<int>", 5);

            Sut.Get<int>().Should().Be(5);
        }
    }
}
