using Autofac;
using System;

namespace Framework.Middleware
{
    public class MiddlewareDepencyResolver
    {
        private static IContainer Container { get; set; }

        public static ContainerBuilder StartBuild()
        {
            return new ContainerBuilder();
        }

        public static void Build(ContainerBuilder pBuilder)
        {
            Container = pBuilder.Build();
        }

        public static bool IsInitialized
        {
            get { return Container != null; }
        }

        public object Resolve(Type pTipo)
        {
            try
            {
                if (Container == null || pTipo == null)
                {
                    throw new NotImplementedException("No se han resuelto las dependencias del Middleware");
                }

                return Container.Resolve(pTipo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
