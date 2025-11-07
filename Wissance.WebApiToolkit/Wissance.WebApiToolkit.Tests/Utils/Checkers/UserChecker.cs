using System.Collections.Generic;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.Tests.Utils.Checkers
{
    internal static class UserChecker
    {
        public static void Check(UserDto expected, UserDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.FullName, actual.FullName);
            Assert.Equal(expected.OrganizationId, actual.OrganizationId);
        }
    }
}