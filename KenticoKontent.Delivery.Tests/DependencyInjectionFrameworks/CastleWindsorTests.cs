﻿using KenticoKontent.Delivery.Tests.DependencyInjectionFrameworks.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace KenticoKontent.Delivery.Tests.DependencyInjectionFrameworks
{
    [Collection("DI Tests")]
    public class CastleWindsorTests
    {
        [Fact]
        public void DeliveryClientIsSuccessfullyResolvedFromCastleWindsorContainer()
        {
            var provider = DependencyInjectionFrameworksHelper
                .GetServiceCollection()
                .BuildWindsorCastleServiceProvider();

            var client = (DeliveryClient)provider.GetService<IDeliveryClient>();

            client.AssertDefaultDependencies();
        }

        [Fact]
        public void DeliveryClientIsSuccessfullyResolvedFromCastleWindsorContainer_CustomModelProvider()
        {
            var provider = DependencyInjectionFrameworksHelper
                .GetServiceCollection()
                .RegisterInlineContentItemResolvers()
                .AddScoped<IModelProvider, FakeModelProvider>()
                .BuildWindsorCastleServiceProvider();

            var client = (DeliveryClient)provider.GetService<IDeliveryClient>();

            client.AssertDefaultDependenciesWithModelProviderAndInlineContentItemTypeResolvers<FakeModelProvider>();
        }
    }
}