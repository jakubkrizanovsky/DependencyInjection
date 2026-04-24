# Dependency Injection
Simple dependency injection framework package for Unity engine

Features:
- **Automatic service discovery** - Simply add the `[Service]` attribute to your class and the system will pick it up as a service from you scene
- **Field and property injection** - Add the `[Inject]` attribute to your fields/properties and they will be injected with the registered services
- **Lifecycle-Safe Injection** - Dependencies are guaranteed to be  available during the `Awake()` and `OnEnable()` methods
- **Interface resolution** - Services also get registered for all of their interfaces and these types can be used to resolve them
- **Lazy injection** - Use the `LazyInject<T>` wrapper type to resolve dependencies on-demand
- **Third-party service support** - Use the `ServiceProxy` component to register services from other packages/third party libraries

## Basic Usage
### 1. Mark a class as service and add it to your scene

``` C#
[Service]
public class ServiceA : MonoBehaviour
{
    public void DoSomething() {
        // Do something
    }
}
```

### 2. Use your service in other components in your scene

``` C#
public class ConsumerA : MonoBehaviour
{
    [Inject]
    private ServiceA _serviceA;

    private void Awake() {
        _serviceA.DoSomething();
    }
}
```


## Installation

Add the following line to the dependencies section of your project's `manifest.json` file. Replace `1.1.1` with the version you want to install.
``` json
"com.jakubkrizanovsky.dependencyinjection": "git+https://github.com/jakubkrizanovsky/DependencyInjection#1.1.1"
```

Alternatively, you can add the Git URL directly through Unity’s Package Manager → Add package from Git URL.


## Setup
Add an object with the `GlobalDependencyInjectionContext` component to some starting scene. It uses `DontDestroyOnLoad` and will stay loaded even while changing scenes.

Also add an object with the `SceneDependencyInjectionContext` component to every other scene, where you wish to register services or inject dependencies.


## How it works
The `DependencyInjectionContext` in your scene will scan your scene hierarchy on its `Awake()`. Using reflection it will detect and register any services that it finds and then inject them into the right fields. It uses `[DefaultExecutionOrder(-1000)]` in order to be the first script that runs in the scene.

Multiple services of each type can be registered, as long as they are managed by different context. Otherwise, only the last registered service of a type will be resolvable from a single context. 

By default, services from the same scene as the object will be resolved first. If the service cannot be found within the context of the scene, the global context will be searched next followed by contexts of all other loaded scenes.

When the context gets destroyed (e.g. scene gets unloaded), it will automatically unregister itself and services it has registered will no longer be resolved.

## Advanced Usage

### Interface Registration
All services are also registered as all of their interfaces and can be resolved as such. 

``` C#
public interface IServiceB
{
    public void DoStuff();
}

[Service]
public class ServiceB : MonoBehaviour, IServiceB
{
    public void DoStuff() {
        // Do some stuff
    }
}

public class ConsumerB : MonoBehaviour
{
    [Inject]
    private IServiceB _serviceB;

    private void Awake() {
        _serviceB.DoStuff();
    }
}

```

### Lazy Inject
The `LazyInject<T>` wrapper class allows you to resolve dependencies on demand, in case they aren't available during the scene scan, but registered later. The first time the service is requested, the dependecy will be resolved. 

You still need to use the `[Inject]` attribute on your field. Use the `Value` property to resolve and access the service.

``` C#
public class ConsumerA : MonoBehaviour
{
    [Inject]
    private LazyInject<ServiceA> _serviceA;

    public void OnSomeEvent() {
        _serviceA.Value.DoSomething();
    }
}
```

### Instantiate and Inject
The context only automatically registers services and injects objects that are in the scene during its `Awake()`, so other objects added later must be handled in some other way. 

The `InstantiateAndInject()` extension method is one of those ways useful when instantiating prefabs. It will instantiate the prefab and automatically inject its fields. It also guarantees the services are available during the `Awake()` and `OnEnable()` methods, using the "inactive parent" technique. 

``` C#
public class Enemy : MonoBehaviour
{
    [Inject]
    private ServiceA _serviceA;

    private void Awake() {
        _serviceA.DoSomething();
    }
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Enemy _enemyPrefab;

    public Enemy SpawnEnemy(Vector3 position) {
        Enemy enemy = this.InstantiateAndInject(_enemyPrefab);
        enemy.transform.position = position;
        return enemy;
    }
}
```

The `InstantiateAndInject()` method will only inject fields on the script that is the type of the prefab during instatiation. If you need to inject fields in all components, use the `InstantiateAndInjectAll()` method instead. This will scan the hierarchy of the prefab during instatiation and inject all the fields in all component. It is however slower due to this.

### Service Proxy
To register services from third-party packages, the `ServiceProxy` component exists. Simply add it to the same GameObject as the service and assign its `_service` attribute (will happen automatically in most cases). It will then be picked up by the framework as any other service would.

### Unique Services
Sometimes you don't want to have multiple instances of the same service registered and starting up (essentially, you want a singleton behavior from your service). That is when the `Unique` parameter of the `[Service]` attribute comes in. By setting `Unique = true` you only limit yourself to having one instance of a service of a type at a time (managed by a single context). Any additional services of the same type will not be registered and will be destroyed before they have a chance to call their `Awake()` or `Start()` methods.

``` C#
[Service(Unique = true)]
public class UniqueService : MonoBehaviour
{
    public void DoSomething() {
        // Do something
    }
}
```

### Persistent Services
To make life easier, the `[Service]` attribute takes an optional parameter `Persistent` that defaults to `false`. If you set it to `true` the service will be added to `DontDestroyOnLoad` during the registration.

``` C#
[Service(Persistent = true)]
public class PersistentService : MonoBehaviour
{
    public void DoSomething() {
        // Do something
    }
}
```

Persistent services are unique by default, this can be overriden by manually specifying `Unique = false` in the `[Service]` attribute parameters.

Persistent services are always registered at the global context, to make them resolvable even after unloading the rest of the scene they were originally in.

### Manual Injection and Service Registation
Sometimes, the tools of the framework might not be enough to handle injection or service registration automatically. In those cases, it can be done manually. 

To manually inject fields of a Component, simply use the `DependencyInjector.Inject()` static method.

``` C#
public class ManualConsumer : MonoBehaviour
{
    [Inject]
    private ServiceA _serviceA;

    private void Awake() {
        DependencyInjector.Inject(this);
        _serviceA.DoSomething();
    }
}
```

To manually register a service, simply use the `RegisterService()` method of the `DependencyInjector` class. It will automatically search for the best context to register it at. You can also set the optional `global` bool parameter to register it at the global context.

``` C#
public class ManualService : MonoBehaviour
{
    public void RegisterService() {
        DependencyInjector.RegisterService(this);
    }
}
```

You can also manually resolve registered services on demand.

``` C#
public static class SomeUtil
{
    public static void DoSomething() {
        ServiceA serviceA = DependencyInjector.Resolve();
        serviceA.DoSomething();
        // Do some more stuff
    }
}
```
