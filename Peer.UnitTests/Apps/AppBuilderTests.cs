using System;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Peer.Apps.AppBuilder;
using Peer.Domain.Configuration;
using wimm.Secundatives;
using Xunit;

namespace Peer.UnitTests.Apps;

public class AppBuilderTests
{
    private static IServiceCollection _services;
    
    public class WithParseTimeConfig
    {
        [Fact]
        public void ConfigIsCalledWithoutRunningApp()
        {
            var called = false;
            var underTest = Construct()
                .WithParseTimeServiceConfig(
                    sc =>
                    {
                        called = true;
                        return new Result<IServiceCollection, ConfigError>(sc.AddSingleton<object>(100));
                    });

            underTest.WithVerb<Doot>(_ => { });
            underTest.Build();
            Assert.True(called);
        }
    }

    public class WithSharedServiceConfig
    {
        [Fact]
        public void ConfigIsNotCalledWhenAppBuilt()
        {
            var called = false;
            var underTest = Construct();
            underTest.WithSharedServiceConfig(
                sp =>
                {
                    called = true;
                    return new Result<IServiceCollection, ConfigError>(sp);
                });

            underTest.WithVerb<Doot>(_ => { });
            var _ = underTest.Build();
            
            Assert.False(called);
        }

        [Fact]
        public async Task ConfigIsCalledWhenAppIsRun()
        {
            var called = false;
            var underTest = Construct();
            underTest.WithSharedServiceConfig(
                sp =>
                {
                    called = true;
                    return new Result<IServiceCollection, ConfigError>(sp);
                });

            underTest.WithVerb<Doot>(_ => { });
            var built = underTest.Build();

            await built.RunAsync(new[] { "doot" });
            
            Assert.True(called);
        }
    }

    public class WithVerb
    {
        [Fact]
        public void TypeIsNotAnnotatedWithVerbAttribute_Throws()
        {
            var underTest = Construct();
            Assert.Throws<ArgumentException>(() => underTest.WithVerb<string>(_ => { }));
        }

        [Fact]
        public void TypeIsAnnotatedWithVerbAttribute_Succeeds()
        {
            var underTest = Construct();
            underTest.WithVerb<Doot>(_ => { });
        }
    }


    private static AppBuilder Construct()
    {
        _services = new ServiceCollection();
        return new AppBuilder(_services);
    }

    [Verb("doot", isDefault: true, HelpText = "The joyous sound of a skeleton")]
    private class Doot
    {
        
    }
}
