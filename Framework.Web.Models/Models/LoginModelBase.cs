using System.ComponentModel.DataAnnotations;

namespace Framework.Web.Models.Model
{
    public abstract class LoginModelBase<T>
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Clave { get; set; }

        public T UsuarioLogueado { get; set; }

        public bool Recordarme { get; set; } = false;

        public string ErrorMesg { get; set; } = string.Empty;
    }
}
