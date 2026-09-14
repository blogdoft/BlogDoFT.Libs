using BlogDoFT.Libs.Flagr.Abstractions;
using FlagrExample.Features;
using FlagrExample.Features.Impl;
using FlagrExample.Flags;
using System;
using System.Collections.Generic;

namespace FlagrExample.Providers.Impl
{
    public class FeatureProvider : IFeatureProvider
    {
        private readonly Dictionary<FeatureFlag, Func<IMyFeature>> _features;
        private readonly IFlagEvaluator _flagrResolver;
        private readonly string _applicationName;

        public FeatureProvider(IFlagEvaluator flagrResolver, string applicationName)
        {
            _flagrResolver = flagrResolver;
            _features = new Dictionary<FeatureFlag, Func<IMyFeature>>
            {
                { FeatureFlag.Unknow, () => new FeatureUnknow(new FeatureOne()) },
                { FeatureFlag.Feature1, () => new FeatureOne() },
                { FeatureFlag.Feature2, () => new FeatureTwo() },
            };
            _applicationName = applicationName;
        }

        public IMyFeature GetFeature()
        {
            var entityContext = new
            {
                ApplicationName = _applicationName,
            };

            var response = _flagrResolver.EvaluateFlag<FeatureFlag>(entityContext);
            var flag = ParseFlag(response);

            var getFlag = _features.GetValueOrDefault(flag);
            return getFlag();
        }

        private FeatureFlag ParseFlag(EvaluationResponse response)
        {
            var variantKey = response.VariantKey;
            if (string.IsNullOrEmpty(variantKey))
                return FeatureFlag.Unknow;

            Enum.TryParse(typeof(FeatureFlag), response.VariantKey, ignoreCase: true, out var flag);
            return (FeatureFlag)flag;
        }
    }
}
