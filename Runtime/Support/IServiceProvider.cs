namespace JakubKrizanovsky.DependencyInjection
{
    public interface IServiceProvider
    {
        public object Service {get;}
        public bool Persistent => false;
    }
}
