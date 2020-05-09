using Framework.Web.Models.Model;
using Framework.Utils.Recursos;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Framework.Web.MVC.Controllers
{
    public abstract class LoginControllerBase<TModel, T> : Controller
        where TModel : LoginModelBase<T>, new()
        where T : class, new()
    {
        private readonly string _loginPageViewName = "Index";
        private readonly string _userIdPropertyName = string.Empty;
        private readonly string _userFullNamePropertyName = string.Empty;

        protected TModel Model;

        public LoginControllerBase(string pLoginPageViewName, string pUserIdPropertyName, string pUserFullNamePropertyName)
        {
            _loginPageViewName = pLoginPageViewName;
            _userIdPropertyName = pUserIdPropertyName;
            _userFullNamePropertyName = pUserFullNamePropertyName;

            ValidarPropiedades();
        }

        private void ValidarPropiedades()
        {
            TModel modelo = new TModel
            {
                UsuarioLogueado = new T()
            };

            object property = null;

            property = modelo.UsuarioLogueado.GetType().GetProperty(_userIdPropertyName);
            if (property == null)
                throw new NotSupportedException(string.Format("La propiedad {0} no es válida", _userIdPropertyName));

            property = modelo.UsuarioLogueado.GetType().GetProperty(_userFullNamePropertyName);
            if (property == null)
                throw new NotSupportedException(string.Format("La propiedad {0} no es válida", _userFullNamePropertyName));
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }

            return View(_loginPageViewName);
        }

        public virtual ActionResult DoLogin(TModel pModelo)
        {
            Model = pModelo;

            if (Model == null || !ModelState.IsValid)
            {
                return RetornarError(Vistas.UsuarioClaveObligatorio);
            }

            pModelo.UsuarioLogueado = GetUserFromDataBase();

            if (pModelo.UsuarioLogueado == null)
                return OnLoginError(Vistas.UsuarioNoEncontrado);

            return GenerateCookieAndRedirect();
        }

        private ActionResult GenerateCookieAndRedirect()
        {
            try
            {
                object id = null;
                object fullName = null;

                id = Model.UsuarioLogueado.GetType().GetProperty(_userIdPropertyName).GetValue(Model.UsuarioLogueado);
                fullName = Model.UsuarioLogueado.GetType().GetProperty(_userFullNamePropertyName).GetValue(Model.UsuarioLogueado);

                FormsAuthentication.SetAuthCookie(fullName.ToString(), false);

                var cookie = new HttpCookie(Utils.InfoUsuarioCookieName);
                cookie.Values.Add(Utils.IdUsuarioCookieKey, id.ToString());
                Response.Cookies.Add(cookie);

                return OnLoginSuccess();
            }
            catch(Exception ex)
            {
                throw new Exception("Error al generar las cookies", ex);
            }
        }

        private ActionResult RetornarError(string pErrorMesg)
        {
            Model.ErrorMesg = pErrorMesg;
            return View(_loginPageViewName, Model);
        }

        public abstract ActionResult OnLoginSuccess();

        public abstract ActionResult OnLoginError(string pErrorMsg);

        public abstract T GetUserFromDataBase();
    }
}
