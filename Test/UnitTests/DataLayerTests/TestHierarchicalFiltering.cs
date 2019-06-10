﻿// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.SeedDemo.Internal;
using Test.FakesAndMocks;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.DataLayerTests
{
    public class TestHierarchicalFiltering
    {
        private ITestOutputHelper _output;

        public TestHierarchicalFiltering(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("1|", 10)]
        [InlineData("1|2|", 9)]
        [InlineData("1|2|3|", 4)]
        [InlineData("1|2|3|6*", 1)]
        public void TestFilterTenantsOk(string dataKey, int expectedCount)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<AppDbContext>();
            using (var context = new AppDbContext(options, new FakeGetClaimsProvider("userId", dataKey)))
            {
                context.Database.EnsureCreated();
                context.AddCompanyAndChildrenInDatabase();

                //ATTEMPT
                var tenants = context.TenantItems.ToList();

                //VERIFY
                foreach (var line in tenants)
                {
                    _output.WriteLine($"\"{line}\",");
                }
                tenants.Count.ShouldEqual(expectedCount);
            }
        }

        
    }
}