using Framework.Utils.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.ContractBase.Contract
{
    public interface ICRUDContractBase<T>
    {
        Task<ExecutionResult> UpdateAsync(T pItem, int pId);
        Task<ExecutionResult> AddAsync(T pItem);
        Task<ExecutionResult> AddBulkAsync(List<T> pItems);
        Task<ExecutionResult> DeleteAsync(int pId);
        Task<List<T>> GetAllAsync();
        Task<List<T>> FilterAsync(Func<T, bool> pFilter);
        Task<T> GetByIdAsync(int pId);

        ExecutionResult Update(T pItem, int pId);
        ExecutionResult Add(T pItem);
        ExecutionResult AddBulk(List<T> pItems);
        ExecutionResult Delete(int pId);
        List<T> GetAll();
        List<T> Filter(Func<T, bool> pFilter);
        T GetById(int pId);
    }

    public interface ICRUDContractBase<T, D> : ICRUDContractBase<T>
        where D : class
    {
        Task<List<D>> Filter_dto_Async(Func<T, bool> pFilter);
        Task<List<D>> GetAll_dto_Async();

        List<D> GetAll_dto();
        List<D> Filter_dto(Func<T, bool> pFilter);
    }
}
