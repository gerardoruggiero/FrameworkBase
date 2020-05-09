using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Framework.Utils
{
    public class Utils
    {
        public static List<string> IsValid(object pObject)
        {
            List<string> mensajesError = new List<string>();

            ValidationContext context = new ValidationContext(pObject);
            IList<ValidationResult> errors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(pObject, context, errors, true))
            {
                foreach (ValidationResult result in errors)
                {
                    mensajesError.Add(result.ErrorMessage);
                }
            }

            return mensajesError;
        }
    }
}
