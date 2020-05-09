using Framework.ContractBase.Contract;
using Framework.Utils.Results;
using System.Collections.Generic;

namespace Framework.Negocio.Negocio
{
    public interface INegocioBase<T> : ICRUDContractBase<T>
    {
        ExecutionResult IsValid(T pItem);

        ExecutionResult CreateBusinessResult(EExecutionResultCode pCode, List<string> pErrorMsg);

        ExecutionResult CreateBusinessResult(EExecutionResultCode pCode, string pErrorMsg = "");
    }

    public interface INegocioBase<T, DTO> : INegocioBase<T>, ICRUDContractBase<T, DTO>
    where DTO : class
    {
    }
}
