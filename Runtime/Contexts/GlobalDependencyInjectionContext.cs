using UnityEngine;

namespace JakubKrizanovsky.DependencyInjection
{
	[DefaultExecutionOrder(-1000)]
	[Service]
    public class GlobalDependencyInjectionContext : ADependencyInjectorContext
    {
		protected override void Awake() {
			base.Awake();
			transform.parent = null;
            DontDestroyOnLoad(gameObject);
		}
    }
}