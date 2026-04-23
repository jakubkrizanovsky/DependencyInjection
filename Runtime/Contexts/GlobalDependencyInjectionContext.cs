using UnityEngine;

namespace JakubKrizanovsky.DependencyInjection
{
	[DefaultExecutionOrder(-1000)]
	[Service(Persistent = true)]
    public class GlobalDependencyInjectionContext : ADependencyInjectorContext
    {
		protected override void Awake() {
			if(DependencyInjector.TryResolve(out GlobalDependencyInjectionContext existingContext)) {
				existingContext.DiscoverAndInject(gameObject.scene); // Discover again using the existing context
				return; // Do not register or discover using this context, it should already be destroyed by now anyway
			}

			base.Awake();
			transform.parent = null;
            DontDestroyOnLoad(gameObject);
		}
    }
}