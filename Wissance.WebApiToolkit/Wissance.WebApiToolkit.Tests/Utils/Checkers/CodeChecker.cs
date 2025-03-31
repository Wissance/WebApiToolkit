using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.TestApp.WebServices.Grpc.Generated;

namespace Wissance.WebApiToolkit.Tests.Utils.Checkers
{
    internal static class CodeChecker
    {
        public static void Check(CodeDto expected, CodeDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Code, actual.Code);
        }

        public static void Check(Code expected, Code actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Code_, actual.Code_);
        }
    }
}