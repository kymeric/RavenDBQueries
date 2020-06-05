using System;
using System.Linq.Expressions;
using Lambda2Js;
using Microsoft.Extensions.Logging;

namespace RavenDB.Queries.Conversions
{
    public class StringConversionExtensions : JavascriptConversionExtension
    {
        private readonly ILogger<StringConversionExtensions> _logger;

        public StringConversionExtensions(ILogger<StringConversionExtensions> logger)
        {
            _logger = logger;
        }
        
        public override void ConvertToJavascript(JavascriptConversionContext context)
        {
            if (context.Node is MemberExpression me && me.Member.DeclaringType == typeof(string))
            {
                if (me.Member.Name == nameof(String.Empty.Length))
                {
                    context.PreventDefault();
                    context.Visitor.Visit(me.Expression);
                    context.Write(".length");
                }
            }
        }
    }
}