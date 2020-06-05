using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace RavenDB.Queries.Tests
{
    public class StringProjectionTests : UnitTests
    {
        public StringProjectionTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void StringLengthReturnsLowerCaseLength()
        {
            var js = Builder.GetJavascriptFunction((string name) => name.Length);
            AssertJS("function(name){return name.length;}", js);
        }

        [Fact]
        public void StringCountReturnsLowerCaseLength()
        {
            var js = Builder.GetJavascriptFunction((string name) => name.Count());
            AssertJS("function(name){return name.length;}", js);
        }
    }
}