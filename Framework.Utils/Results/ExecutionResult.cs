using System.Collections.Generic;

namespace Framework.Utils.Results
{
    public class ExecutionResult
    {
        public List<string> ErrorMsg { get; set; }

        public EExecutionResultCode ResultCode { get; set; }
    }
}
