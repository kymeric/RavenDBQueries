using System;
using System.Collections.Generic;
using System.Linq;
using Lambda2Js;
using Xunit;
using Xunit.Abstractions;

namespace RavenDB.Queries.Tests
{
    public class MultipleParameterProjectionTests : UnitTests
    {
        private class Thing
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        protected override RavenQueryBuilder Builder
        {
            get
            {
                var ret = base.Builder;
                ret.ConversionExtensions.Add(new MemberInitAsJson(t => t.Assembly == typeof(MultipleParameterProjectionTests).Assembly));
                return ret;
            }
        }

        public MultipleParameterProjectionTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void StringAndIntToThing()
        {
            var js = Builder.GetJavascriptFunction((string name, int value) => new Thing() {Name = name, Value = value});
            AssertJS("function(name,value){return {Name:name,Value:value};}", js);
        }

        [Fact]
        public void StringAndIntToAnonymousType()
        {
            var js = Builder.GetJavascriptFunction((string name, int value) => new {Name = name, Value = value});
            AssertJS("function(name,value){return {Name:name,Value:value};}", js);
        }

        [Fact]
        public void ThingToName()
        {
            var js = Builder.GetJavascriptFunction((Thing thing) => thing.Name);
            AssertJS("function(thing){return thing.Name;}", js);
        }
    }
}