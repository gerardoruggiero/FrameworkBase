using Framework.Negocio.Negocio;
using Framework.Middleware.Contract;
using Framework.Utils.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Middleware.Model
{
    public abstract class MiddlewareBase<T, N> : IMiddleware<T>
            where T : class
            where N : INegocioBase<T>
    {
        private N _negocio;

        public MiddlewareBase()
        {
            ResolverNegocio();
        }

        private void ResolverNegocio()
        {
            string fullName = typeof(N).FullName;
            Type baseType = Type.GetType(fullName.Replace("Model", "Contract").Replace(typeof(N).Name, "I" + typeof(N).Name) + ", " + typeof(N).Assembly);

            if (baseType == null)
            {
                throw new NotImplementedException("El contrato del negocio no es válido");
            }

            _negocio = (N)new MiddlewareDepencyResolver().Resolve(baseType);
        }

        public virtual N Negocio { get { return _negocio; } }

        public ExecutionResult Add(T pItem)
        {
            return Negocio.Add(pItem);
        }

        public async Task<ExecutionResult> AddAsync(T pItem)
        {
            return await Negocio.AddAsync(pItem);
        }

        public ExecutionResult AddBulk(List<T> pItems)
        {
            return Negocio.AddBulk(pItems);
        }

        public async Task<ExecutionResult> AddBulkAsync(List<T> pItems)
        {
            return await Negocio.AddBulkAsync(pItems);
        }

        public ExecutionResult CreateBusinessResult(EExecutionResultCode pCode, string pErrorMsg = "")
        {
            return Negocio.CreateBusinessResult(pCode, pErrorMsg);
        }

        public ExecutionResult Delete(int pId)
        {
            return Negocio.Delete(pId);
        }

        public async Task<ExecutionResult> DeleteAsync(int pId)
        {
            return await Negocio.DeleteAsync(pId);
        }

        public List<T> Filter(Func<T, bool> pFilter)
        {
            return Negocio.Filter(pFilter);
        }

        public async Task<List<T>> FilterAsync(Func<T, bool> pFilter)
        {
            return await _negocio.FilterAsync(pFilter);
        }

        public List<T> GetAll()
        {
            return Negocio.GetAll();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await Negocio.GetAllAsync();
        }

        public T GetById(int pId)
        {
            return Negocio.GetById(pId);
        }

        public async Task<T> GetByIdAsync(int pId)
        {
            return await Negocio.GetByIdAsync(pId);
        }

        public ExecutionResult IsValid(T pItem)
        {
            return Negocio.IsValid(pItem);
        }

        public ExecutionResult Update(T pItem, int pId)
        {
            return Negocio.Update(pItem, pId);
        }

        public async Task<ExecutionResult> UpdateAsync(T pItem, int pId)
        {
            return await Negocio.UpdateAsync(pItem, pId);
        }

        public ExecutionResult CreateBusinessResult(EExecutionResultCode pCode, List<string> pErrorMsg)
        {
            return Negocio.CreateBusinessResult(pCode, pErrorMsg);
        }
    }
}
