using System.Collections.Generic;

namespace Framework.Security.Autorization
{
    public interface IAuthorization
    {
        bool IsAutorized(Dictionary<string, object> pValues);

        bool IsAutorized(string pControllerName, string pMethodName, object pUserId);
    }
}
