using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lambda2Js;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using RavenDB.Queries.Conversions;

namespace RavenDB.Queries
{
    public class RavenQueryBuilder
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ILoggerFactory _loggerFactory;

        //Properties and Defaults for Builder
        public JsCompilationFlags Flags { get; set; } = 0;

        public List<JavascriptConversionExtension> ConversionExtensions { get; } = new List<JavascriptConversionExtension>();
        public bool AddRavenDBExtensions { get; set; } = true;

        public RavenQueryBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            //TODO: Better logging and IOC handling
            ConversionExtensions.Add(new StringConversionExtensions(_loggerFactory.CreateLogger<StringConversionExtensions>()));
        }
        
        public string GetJavascriptFunction<T, TRet>(Expression<Func<T, TRet>> expression)
        {
            return expression.CompileToJavascript(GetOptions());
        }
        
        public string GetJavascriptFunction<T1, T2, TRet>(Expression<Func<T1, T2, TRet>> expression)
        {
            return expression.CompileToJavascript(GetOptions());
        }
        
        public string GetJavascriptFunction<T1, T2, T3, TRet>(Expression<Func<T1, T2, T3, TRet>> expression)
        {
            return expression.CompileToJavascript(GetOptions());
        }

        public string GetJavascriptFunction<T1, T2, T3, T4, TRet>(Expression<Func<T1, T2, T3, T4, TRet>> expression)
        {
            return expression.CompileToJavascript(GetOptions());
        }


        private JavascriptCompilationOptions GetOptions(JsCompilationFlags flags = 0)
        {
            var extensions = new List<JavascriptConversionExtension>();
            if (ConversionExtensions.Count > 0)
                extensions.AddRange(ConversionExtensions);

            if (AddRavenDBExtensions)
            {
                foreach (var ravenExt in GetRavenExtensions())
                {
                    extensions.Add(ravenExt);
                }
            }

            return new JavascriptCompilationOptions(
                Flags,
                ScriptVersion.Es51,
                extensions.ToArray());
        }

        //TODO: Make configurable?
        private static readonly string[] IgnoredRavenExtensions = new []
        {
            "WrappedConstantSupport`1", "ReplaceParameterWithNewName", "SubscriptionsWrappedConstantSupport", "ConstSupport", "IdentityPropertySupport"
        };

        private List<JavascriptConversionExtension> GetRavenExtensions()
        {
            var ass = typeof(IDocumentStore).Assembly;
            var jsce = ass.GetTypes().Single(t => t.Name == "JavascriptConversionExtensions");
            var ret = new List<JavascriptConversionExtension>();
            foreach (var nt in jsce.GetNestedTypes().Where(t => typeof(JavascriptConversionExtension).IsAssignableFrom(t)))
            {
                try
                {
                    var constructor = nt.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
                    if (constructor != null)
                    {
                        var et = (JavascriptConversionExtension) constructor.Invoke(null);
                        ret.Add(et);
                    }
                    else
                    {
                        if (!IgnoredRavenExtensions.Contains(nt.Name))
                            throw new InvalidOperationException($"Couldn't find no-arg constructor for: {nt.Name}");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Unable to create instance of: {nt.Name}", ex);
                }
            }

            return ret;
        }
    }
}