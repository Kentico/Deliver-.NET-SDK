﻿using Kentico.Kontent.Delivery.Builders.DeliveryOptions;
using Kentico.Kontent.Delivery.InlineContentItems;
using Kentico.Kontent.Delivery.RetryPolicy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Kentico.Kontent.Delivery.Builders.DeliveryClient
{
    internal sealed class DeliveryClientBuilderImplementation : IDeliveryClientBuilder, IOptionalClientSetup
    {
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();
        private Delivery.DeliveryOptions _deliveryOptions;

        public IOptionalClientSetup BuildWithDeliveryOptions(Func<IDeliveryOptionsBuilder, Delivery.DeliveryOptions> buildDeliveryOptions)
        {
            var builder = DeliveryOptionsBuilder.CreateInstance();
            _deliveryOptions = buildDeliveryOptions(builder);

            return this;
        }

        public IOptionalClientSetup BuildWithProjectId(string projectId)
            => BuildWithDeliveryOptions(builder =>
                builder
                    .WithProjectId(projectId)
                    .UseProductionApi()
                    .Build());

        public IOptionalClientSetup BuildWithProjectId(Guid projectId)
            => BuildWithDeliveryOptions(builder =>
                builder
                    .WithProjectId(projectId)
                    .UseProductionApi()
                    .Build());

        IOptionalClientSetup IOptionalClientSetup.WithHttpClient(HttpClient httpClient)
            => RegisterOrThrow(httpClient, nameof(httpClient));

        IOptionalClientSetup IOptionalClientSetup.WithContentLinkUrlResolver(IContentLinkUrlResolver contentLinkUrlResolver)
            => RegisterOrThrow(contentLinkUrlResolver, nameof(contentLinkUrlResolver));

        IOptionalClientSetup IOptionalClientSetup.WithInlineContentItemsResolver<T>(IInlineContentItemsResolver<T> inlineContentItemsResolver)
            => RegisterInlineContentItemsResolverOrThrow(inlineContentItemsResolver);

        IOptionalClientSetup IOptionalClientSetup.WithInlineContentItemsProcessor(IInlineContentItemsProcessor inlineContentItemsProcessor)
            => RegisterOrThrow(inlineContentItemsProcessor, nameof(inlineContentItemsProcessor));

        IOptionalClientSetup IOptionalClientSetup.WithModelProvider(IModelProvider modelProvider)
            => RegisterOrThrow(modelProvider, nameof(modelProvider));

        IOptionalClientSetup IOptionalClientSetup.WithTypeProvider(ITypeProvider typeProvider)
            => RegisterOrThrow(typeProvider, nameof(typeProvider));

        IOptionalClientSetup IOptionalClientSetup.WithRetryPolicyProvider(IRetryPolicyProvider retryPolicyProvider)
            => RegisterOrThrow(retryPolicyProvider, nameof(retryPolicyProvider));

        IOptionalClientSetup IOptionalClientSetup.WithPropertyMapper(IPropertyMapper propertyMapper)
            => RegisterOrThrow(propertyMapper, nameof(propertyMapper));

        IDeliveryClient IDeliveryClientBuild.Build() //TODO TOHLE S FACTORIES NEPUJDE ????
        {
            _serviceCollection.AddDeliveryClient(_deliveryOptions);
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var client = serviceProvider.GetService<IDeliveryClient>();

            return client;
        }

        private DeliveryClientBuilderImplementation RegisterOrThrow<TType>(TType instance, string parameterName) where TType : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            _serviceCollection.AddSingleton(instance);

            return this;
        }

        private DeliveryClientBuilderImplementation RegisterInlineContentItemsResolverOrThrow<TContentItem>(IInlineContentItemsResolver<TContentItem> inlineContentItemsResolver)
        {
            if (inlineContentItemsResolver == null)
            {
                throw new ArgumentNullException(nameof(inlineContentItemsResolver));
            }

            _serviceCollection.AddDeliveryInlineContentItemsResolver(inlineContentItemsResolver);

            return this;
        }
    }
}
