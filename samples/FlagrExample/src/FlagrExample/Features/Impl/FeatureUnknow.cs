using System;

namespace FlagrExample.Features.Impl
{
    public class FeatureUnknow : IMyFeature
    {
        private readonly IMyFeature _defaultFlag;

        public FeatureUnknow(IMyFeature defaultFlag)
        {
            _defaultFlag = defaultFlag;
        }
        public void Execute()
        {
            Console.WriteLine("Your application has no configured flag!");
            Console.WriteLine("Executing default feature:");
            _defaultFlag.Execute();
        }
    }
}
