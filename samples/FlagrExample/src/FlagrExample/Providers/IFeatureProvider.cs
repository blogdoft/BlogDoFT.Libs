using FlagrExample.Features;

namespace FlagrExample.Providers
{
    public interface IFeatureProvider
    {
        IMyFeature GetFeature();
    }
}
