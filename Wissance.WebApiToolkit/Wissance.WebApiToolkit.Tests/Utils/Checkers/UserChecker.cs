using System.Collections.Generic;
using Wissance.WebApiToolkit.TestApp.Data.Entity;

namespace Wissance.WebApiToolkit.Tests.Utils.Checkers
{
    internal static class UserChecker
    {
        public static void Check(UserEntity expected, UserEntity actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.FullName, actual.FullName);
            Assert.Equal(expected.OrganizationId, actual.OrganizationId);
        }
    }
}