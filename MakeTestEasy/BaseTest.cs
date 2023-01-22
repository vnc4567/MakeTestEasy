using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using System.Reflection;

namespace MakeTestEasy
{
    public class BaseTest<TAppService>
    {
        protected TAppService Sut;
        protected Fixture Fixture;
        private List<Mock> _moqs;
        private List<object> _dependenciesNotToMock = new();

        public BaseTest()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());
            _moqs = new List<Mock>();
            MockConstructorParameters();
        }

        public BaseTest<TAppService> Create()
        {
            Fixture = new Fixture();

            Fixture.Customize(new AutoMoqCustomization());

            _moqs = new List<Mock>();

            return this;
        }

        public BaseTest<TAppService> ExcludeDependenciesToBeMock<T>(T typeToNotMock) where T : class
        {
            Fixture.Inject(typeToNotMock);

            _dependenciesNotToMock.Add(typeToNotMock);

            return this;
        }

        public void Build()
        {
            MockConstructorParameters();
        }

        protected Mock<T> Get<T>() where T : class => _moqs.FirstOrDefault(p => p.Object is T) as Mock<T>;

        private void MockConstructorParameters() => CreateMocks(GetTypesOfConstructorParameter());

        private IReadOnlyList<Type> GetTypesOfConstructorParameter() =>
            typeof(TAppService).GetConstructors()
                               .SelectMany(x => x.GetParameters())
                               .Select(p => p.ParameterType)
                               .ToList();

        public void CreateMocks(IReadOnlyList<Type> typeToMoqs)
        {
            MethodInfo method = typeof(BaseTest<TAppService>).GetMethod("CreateMock");

            foreach (var item in typeToMoqs)
            {
                if (!IsDependenciesNotToMock(item))
                {
                    MethodInfo generic = method.MakeGenericMethod(item);
                    _moqs.Add((Mock)generic.Invoke(null, new object[] { Fixture }));
                }
            }

            Sut = Fixture.Create<TAppService>();
        }

        private bool IsDependenciesNotToMock(Type type)
        {
            bool result = false;

            foreach (var item in _dependenciesNotToMock)
            {
                if (type.IsAssignableFrom(item.GetType()))
                {
                    return true;
                }
            }

            return result;
        }

        public static Mock<T> CreateMock<T>(Fixture fixture) where T : class => fixture.Freeze<Mock<T>>();
    }
}