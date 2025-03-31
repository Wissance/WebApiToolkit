using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.Tests.Utils.Checkers
{
    internal static class OrganizationChecker
    {
        public static void Check(OrganizationDto expected, OrganizationDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.ShortName, actual.ShortName);
            Assert.Equal(expected.TaxNumber, actual.TaxNumber);
        }
    }
}