using Framework.ErrorLog;
using Framework.ErrorLog.Contract;
using Framework.Middleware.Contract;
using Framework.Utils.Results;
using Framework.Web.Models.Model;
using Framework.Utils.Recursos;
using Framework.Security.Autorization;

using System;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Framework.Web.MVC.Controllers
{
    public abstract class BaseController<T, M, MD> : Controller
        where T : class, new()
        where M : ModelBase<T>, new()
        where MD : IMiddleware<T>, new()
    {
        protected const string DEFAULT_INDEX_PAGE = "Index";
        protected const string DEFAULT_CREATE_METHOD = "Create";
        protected const string DEFAULT_EDIT_METHOD = "Edit";
        protected const string DEFAULT_DELETE_METHOD = "Delete";
        protected const string DEFAULT_CREATE_EDIT_VIEW = "CreateOrEdit";
        protected const string DEFAULT_DELETE_VIEW = "Delete";

        private readonly ILLogger _logger;
        private readonly IAuthorization _authorizer;
        protected MD Middleware;

        public M Model { get; }

        protected BaseController(IAuthorization pAuthorizer = null, ILLogger pLogger = null)
        {
            Middleware = new MD();
            _logger = pLogger;
            _authorizer = pAuthorizer;
            Model = new M();
        }

        #region MVC

        protected override void HandleUnknownAction(string actionName)
        {
            throw new InvalidOperationException(actionName);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //Comprobamos que el usuario haya realizado Login al sistema y que no está intentando ingresar por fuera
            if (!User.Identity.IsAuthenticated)
            {
                throw new AuthenticationException();
            }

            if (_authorizer != null)
            {
                string actionName = filterContext.ActionDescriptor.ActionName;
                string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var idUsuario = GetLoggedUserId();

                //Comprobamos que tenga acceso al formulario.
                if (!_authorizer.IsAutorized(controller, actionName, idUsuario))
                {
                    throw new UnauthorizedAccessException(Vistas.Usuario_SinPermisosParaIngresar);
                }
            }
        }

        public virtual async Task<ActionResult> Index(bool pLazy = false)
        {
            try
            {
                if (!pLazy)
                {
                    Model.ItemList = await Middleware.GetAllAsync();
                }
                return View(Model);
            }
            catch (Exception ex)
            {
                LogException("Error al invocar Index", ex);
                throw;
            }
        }

        public virtual async Task<ActionResult> Create()
        {
            return await ResolveActionABM(DEFAULT_CREATE_METHOD, DEFAULT_CREATE_EDIT_VIEW);
        }

        public virtual async Task<ActionResult> Edit(int pId)
        {
            return await ResolveActionABM(DEFAULT_EDIT_METHOD, DEFAULT_CREATE_EDIT_VIEW, pId);
        }

        public virtual async Task<ActionResult> Delete(int pId)
        {
            return await ResolveActionABM(DEFAULT_DELETE_METHOD, DEFAULT_DELETE_VIEW, pId);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Create(M pModel)
        {
            return await RsolveActionPostABM(pModel, DEFAULT_CREATE_METHOD);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Edit(M pModel)
        {
            return await RsolveActionPostABM(pModel, DEFAULT_EDIT_METHOD);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Delete(M pModel)
        {
            return await RsolveActionPostABM(pModel, DEFAULT_DELETE_METHOD);
        }

        private async Task<ActionResult> ResolveActionABM(string pAction, string pView, int pId = 0)
        {
            try
            {
                Model.Action = pAction;
                if (pId == 0)
                {
                    Model.Item = new T();
                }
                else
                {
                    Model.IdItem = pId;
                    Model.Item = await Middleware.GetByIdAsync(pId);
                }

                if (pAction == DEFAULT_CREATE_METHOD || pAction == DEFAULT_EDIT_METHOD)
                {
                    await LoadModelListsAsync(Model);
                }

                return View(pView, Model);
            }
            catch (Exception ex)
            {
                LogException("Error resolviendo ABM: " + pAction + ", " + pView, ex);
                throw;
            }
        }

        private object GetLoggedUserId()
        {
            object idUsuario;
            var cookie = Request.Cookies[Utils.InfoUsuarioCookieName];
            if (cookie != null)
            {
                idUsuario = cookie.Values[Utils.IdUsuarioCookieKey];
                if (idUsuario == null)
                {
                    throw new Exception(Vistas.UsuarioCookieNoId);
                }
            }
            else
            {
                LogMessage("No se logró obtener la cookie con la información de usuario", ELogType.Info);
                throw new CookieException();
            }

            return idUsuario;
        }

        private async Task<ActionResult> RsolveActionPostABM(M pModel, string pAction)
        {
            if (!ModelState.IsValid)
            {
                if (pAction == DEFAULT_CREATE_METHOD || pAction == DEFAULT_EDIT_METHOD)
                {
                    await LoadModelListsAsync(pModel);
                }

                return View(DEFAULT_CREATE_EDIT_VIEW, pModel);
            }

            ExecutionResult resultado;

            switch (pModel.Action)
            {
                case DEFAULT_CREATE_METHOD:
                    resultado = await Middleware.AddAsync(pModel.Item);
                    break;
                case DEFAULT_EDIT_METHOD:
                    resultado = await Middleware.UpdateAsync(pModel.Item, pModel.IdItem);
                    break;
                default:
                    resultado = await Middleware.DeleteAsync(pModel.IdItem);
                    break;
            }

            if (resultado.ResultCode != EExecutionResultCode.OK)
            {
                LogMessage(resultado, ELogType.Info);
                pModel.ErrorMsg = resultado.ErrorMsg;
                return View(DEFAULT_CREATE_EDIT_VIEW, pModel);
            }

            return RedirectToAction(DEFAULT_INDEX_PAGE);
        }

        protected virtual async Task LoadModelListsAsync(M pModel)
        {
            await Task.Run(() => { });
        }

        #endregion MVC

        #region Log

        protected void LogMessage(object pMessage, ELogType pTipo)
        {
            if (_logger != null)
            {
                if (pTipo == ELogType.Error)
                {
                    _logger.LogError(pMessage);
                }
                else if (pTipo == ELogType.Info)
                {
                    _logger.LogMessage(pMessage);
                }
                else if (pTipo == ELogType.Warning)
                {
                    _logger.LogWarning(pMessage);
                }
            }
        }

        protected void LogException(object pMessage, Exception ex)
        {
            _logger?.LogError(pMessage, ex);
        }

        #endregion
    }
}