using System;

namespace FlagrExample.Features.Impl
{
    public class FeatureOne : IMyFeature
    {
        public void Execute()
        {
            Console.WriteLine("You choose the Feature One");
        }
    }
}
