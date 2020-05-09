using System;

namespace Framework.ErrorLog.Contract
{
    public interface ILLogger
    {
        void LogError(object pMessage, Exception pException);

        void LogError(object pMessage);

        void LogWarning(object pMessage);

        void LogMessage(object pMessage);

        void SendError(LLog pLog);

        void SendError(Exception pException);
    }
}
