using Framework.ContractBase.Contract;
using Framework.Utils.Results;
using System.Collections.Generic;

namespace Framework.DataAccess.Contract
{
    public interface IRepositoryBase<T> : ICRUDContractBase<T>
    {
        ExecutionResult CreateDDBBResult(EExecutionResultCode pCode, List<string> pErrorMsg);

        ExecutionResult CreateDDBBResult(EExecutionResultCode pCode, string pErrorMsg = "");
    }

    public interface IRepositoryBase<T, DTO> : IRepositoryBase<T>, ICRUDContractBase<T, DTO>
        where DTO : class
    {
    }
}
