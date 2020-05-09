using Framework.Negocio.Negocio;

namespace Framework.Middleware.Contract
{
    public interface IMiddleware<T> : INegocioBase<T>
    {
    }
}
