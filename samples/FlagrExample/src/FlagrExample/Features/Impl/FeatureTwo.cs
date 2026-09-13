using System;

namespace FlagrExample.Features.Impl
{
    public class FeatureTwo : IMyFeature
    {
        public void Execute()
        {
            Console.WriteLine("You choose the Feature Two");
        }
    }
}
