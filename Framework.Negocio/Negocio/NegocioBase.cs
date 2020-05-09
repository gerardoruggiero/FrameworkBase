using Framework.DataAccess.Contract;
using Framework.Utils.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Framework.Negocio.Negocio
{
    public abstract class NegocioBase<T, D> : INegocioBase<T>
        where T : class
        where D : IRepositoryBase<T>
    {        
        private readonly IRepositoryBase<T> _repositoryBase;
        private readonly bool _hasAssociatedMetadata = false;

        protected NegocioBase(D pDaoBase, bool pHasAssociatedMetadada = false)
        {
            _repositoryBase = pDaoBase;
            _hasAssociatedMetadata = pHasAssociatedMetadada;
        }

        #region Metodos publicos

        public virtual ExecutionResult IsValid(T pItem)
        {
            if (_hasAssociatedMetadata)
            {
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(T)), typeof(T));
            }

            var mensajesError = Utils.Utils.IsValid(pItem);

            if(mensajesError.Count > 0)
            {
                return CreateBusinessResult(EExecutionResultCode.BusinessOperation, mensajesError);
            }

            return CreateBusinessResult(EExecutionResultCode.OK);
        }

        public async virtual Task<ExecutionResult> UpdateAsync(T pItem, int pId)
        {
            return await AddOrUpdateAsync(pItem, pId);
        }

        public async virtual Task<ExecutionResult> AddAsync(T pItem)
        {
            return await AddOrUpdateAsync(pItem);
        }

        public async virtual Task<ExecutionResult> DeleteAsync(int pId)
        {
            try
            {
                await _repositoryBase.DeleteAsync(pId);
                return CreateBusinessResult(EExecutionResultCode.OK);
            }
            catch (Exception ex)
            {
                return CreateBusinessResult(EExecutionResultCode.DeleteError, ex.Message);
            }
        }

        public async virtual Task<List<T>> GetAllAsync()
        {
            return await _repositoryBase.GetAllAsync();
        }

        public async virtual Task<List<T>> FilterAsync(Func<T, bool> pFilter)
        {
            return await _repositoryBase.FilterAsync(pFilter);
        }

        public async virtual Task<T> GetByIdAsync(int pId)
        {
            return await _repositoryBase.GetByIdAsync(pId);
        }

        public virtual async Task<ExecutionResult> AddBulkAsync(List<T> pItems)
        {
            try
            {
                await _repositoryBase.AddBulkAsync(pItems);
                return CreateBusinessResult(EExecutionResultCode.OK);
            }
            catch(Exception ex)
            {
                return CreateBusinessResult(EExecutionResultCode.BulkInsertError, ex.Message);
            }
        }

        public virtual ExecutionResult Update(T pItem, int pId)
        {
            return AddOrUpdate(pItem, pId);
        }

        public virtual ExecutionResult Add(T pItem)
        {
            return AddOrUpdate(pItem);
        }

        public virtual ExecutionResult AddBulk(List<T> pItems)
        {
            try
            {
                _repositoryBase.AddBulk(pItems);
                return CreateBusinessResult(EExecutionResultCode.OK);
            }
            catch (Exception ex)
            {
                return CreateBusinessResult(EExecutionResultCode.BulkInsertError, ex.Message);
            }
        }

        public ExecutionResult Delete(int pId)
        {
            try
            {
                _repositoryBase.Delete(pId);
                return CreateBusinessResult(EExecutionResultCode.OK);
            }
            catch (Exception ex)
            {
                return CreateBusinessResult(EExecutionResultCode.DeleteError, ex.Message);
            }
        }

        public virtual List<T> GetAll()
        {
            return _repositoryBase.GetAll();
        }

        public virtual List<T> Filter(Func<T, bool> pFilter)
        {
            return _repositoryBase.Filter(pFilter);
        }

        public virtual T GetById(int pId)
        {
            return _repositoryBase.GetById(pId);
        }

        #endregion

        #region Metodos privados
        private async Task<ExecutionResult> AddOrUpdateAsync(T pItem, int pId = 0)
        {
            try
            {
                var result = IsValid(pItem);
                if (result.ResultCode == EExecutionResultCode.OK)
                {
                    if (pId != 0)
                    {
                        await _repositoryBase.UpdateAsync(pItem, pId);
                    }
                    else
                    {
                        await _repositoryBase.AddAsync(pItem);
                    }

                    return CreateBusinessResult(EExecutionResultCode.OK);
                }
                return CreateBusinessResult(EExecutionResultCode.BusinessOperation, result.ErrorMsg);
            }
            catch(Exception ex)
            {
                return CreateBusinessResult(EExecutionResultCode.BusinessOperation, ex.Message);
            }
        }

        private ExecutionResult AddOrUpdate(T pItem, int pId = 0)
        {
            try
            {
                var result = IsValid(pItem);

                if (result.ResultCode == EExecutionResultCode.OK)
                {
                    if (pId != 0)
                    {
                        _repositoryBase.Update(pItem, pId);
                    }
                    else
                    {
                        _repositoryBase.Add(pItem);
                    }

                    return CreateBusinessResult(EExecutionResultCode.OK);
                }
                return CreateBusinessResult(EExecutionResultCode.BusinessOperation, result.ErrorMsg);
            }
            catch (Exception ex)
            {
                return CreateBusinessResult(EExecutionResultCode.AddOrUpdateError, ex.Message);
            }
        }

        #endregion

        public ExecutionResult CreateBusinessResult(EExecutionResultCode pCode, string pErrorMsg = "")
        {
            List<string> errores = new List<string>();
            if (string.IsNullOrEmpty(pErrorMsg) == false)
                errores.Add(pErrorMsg);

            return new ExecutionResult() { ErrorMsg = errores, ResultCode = pCode };
        }

        public ExecutionResult CreateBusinessResult(EExecutionResultCode pCode, List<string> pErrorMsg)
        {
            return new ExecutionResult() { ErrorMsg = pErrorMsg, ResultCode = pCode };
        }
    }

    public abstract class NegocioBase<T, D, DTO> : NegocioBase<T, D>, INegocioBase<T, DTO>
        where T : class
        where D : IRepositoryBase<T, DTO>
        where DTO : class
    {
        private readonly D _repositoryBase;

        protected NegocioBase(D pDaoBase, bool pHasAssociatedMetadada = false) 
            : base(pDaoBase, pHasAssociatedMetadada)
        {
            _repositoryBase = pDaoBase;
        }

        public virtual List<DTO> Filter_dto(Func<T, bool> pFilter)
        {
           return  _repositoryBase.Filter_dto(pFilter);
        }

        public virtual async Task<List<DTO>> Filter_dto_Async(Func<T, bool> pFilter)
        {
            return await _repositoryBase.Filter_dto_Async(pFilter);
        }

        public virtual List<DTO> GetAll_dto()
        {
            return _repositoryBase.GetAll_dto();
        }

        public virtual async Task<List<DTO>> GetAll_dto_Async()
        {
            return await _repositoryBase.GetAll_dto_Async();
        }        
    }
}
