using System.Linq;
using UnityEngine;

namespace JakubKrizanovsky.DependencyInjection
{
    public class ServiceProxy : MonoBehaviour, IServiceProvider
    {
        [SerializeField]
        private MonoBehaviour _service;
        public object Service => _service;

		private void OnValidate() {
            if(_service == null) Debug.LogError($"[ServiceRegisterer({name})] _service is null", this);
        }

        private void Reset() {
            _service = GetComponents<MonoBehaviour>()
                    .Where(c => c != this)
                    .FirstOrDefault();
        }
	}
}