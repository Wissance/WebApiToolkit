using System.Collections.Generic;
using System.Linq;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.Tests.Utils.Checkers
{
    internal static class RoleChecker
    {
        public static void Check(RoleDto expected, RoleDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equivalent(expected.Users, actual.Users);
        }
        
        public static void Check(IList<RoleDto> expected, IList<RoleDto> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            foreach (RoleDto e in expected)
            {
                RoleDto a = actual.FirstOrDefault(r => r.Id == e.Id);
                Assert.NotNull(a);
                Check(e, a);
            }
        }
    }
}