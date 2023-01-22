using Moq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MakeTestEasy
{
    public static class MoqExtension
    {
        public static void SetupWithoutParamAsync<T, TResult>(this Mock<T> mock, string methodName, TResult result) where T : class
        {
            List<Expression<Func<T, Task<TResult>>>> exp = CreateLambdas<T, Task<TResult>>(methodName);

            foreach (var item in exp)
            {
                mock.Setup(item).ReturnsAsync(result);
            }
        }

        public static void SetupWithoutParam<T, TResult>(this Mock<T> mock, string methodName, TResult result) where T : class
        {
            List<Expression<Func<T, TResult>>> exp = CreateLambdas<T, TResult>(methodName);

            foreach (var item in exp)
            {
                mock.Setup(item).Returns(result);
            }
        }

        private static List<Expression<Func<T, TU>>> CreateLambdas<T, TU>(string methodName)
        {
            var methodsInfos = GetMethodInfos<T>(methodName);

            if (methodsInfos == null)
            {
                methodsInfos = typeof(T).GetInterfaces()[0].GetMethods().Where(p => p.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase));
            }

            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");

            return methodsInfos.Select(methodInfo =>
            {
                if (methodInfo.IsGenericMethodDefinition)
                {
                    MethodCallExpression callExp2 = Expression.Call(parameterExpression, methodInfo.MakeGenericMethod(typeof(TU)));

                    return Expression.Lambda<Func<T, TU>>(callExp2, parameterExpression);
                }

                MethodCallExpression callExp = Expression.Call(parameterExpression, methodInfo, methodInfo.GetParameters().Select(p => GenerateItIsAny(p.ParameterType)));

                return Expression.Lambda<Func<T, TU>>(callExp, parameterExpression);

            }).ToList();
        }

        private static IEnumerable<MethodInfo> GetMethodInfos<T>(string methodName)
        {
            Regex regex = new(@"([a-z]+)(?:<.*)", RegexOptions.IgnoreCase);

            var matches = regex.Match(methodName);

            if (matches.Success && matches.Groups.Count > 1)
            {
                return typeof(T).GetMethods().Where(p => p.IsGenericMethodDefinition && p.Name.Equals(matches.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase));
            }

            return typeof(T).GetMethods().Where(p => p.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase));
        }

        private static MethodCallExpression GenerateItIsAny(Type T)
        {
            var itIsAnyT = typeof(It)
                .GetMethod("IsAny")
                .MakeGenericMethod(T);

            return Expression.Call(itIsAnyT);
        }
    }
}
