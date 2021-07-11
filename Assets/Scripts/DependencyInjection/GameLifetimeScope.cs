using VContainer;
using VContainer.Unity;

namespace TDGame.DependencyInjection
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // builder.Register(Lifetime.Singleton);
        }
    }
}