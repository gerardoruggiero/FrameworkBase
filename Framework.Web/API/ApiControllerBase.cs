using Framework.Middleware.Contract;
using Framework.Web.Models.APIModel;
using Framework.Utils.Recursos;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Framework.Middleware;

namespace Framework.Web.API
{
    public abstract class ApiControllerBase<T, M, MD> : ApiController
        where T : class, new()
        where M : ApiModelBase<T>, new()
        where MD : IMiddleware<T>, new()
    {
        protected MD Middleware;
        protected M Model;

        public ApiControllerBase()
        {
            Middleware = new MD();
            Model = new M();

            if (!MiddlewareDepencyResolver.IsInitialized)
            {
                throw new NotImplementedException("No se han resuelto las dependencias del Middleware");
            }
        }

        [HttpGet]
        [Route("GetAsync")]
        public virtual async Task<IHttpActionResult> GetAsync()
        {
            try
            {
                Model.ItemList = await Middleware.GetAllAsync();
            }
            catch (Exception ex)
            {
                Model.ErrorMsg = ex.Message;
            }

            return Json(Model);
        }

        [HttpPost]
        [Route("AddAsync")]
        public virtual async Task<IHttpActionResult> AddAsync([FromBody]T item)
        {
            if (item == null)
            {
                BadRequest(Vistas.ParametroNuloOVacio);
            }

            try
            {
                var msg = await Middleware.AddAsync(item);
                return Ok(msg);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("UpdateAsync")]
        public virtual async Task<IHttpActionResult> UpdateAsync([FromBody]T item, int itemId)
        {
            if (item == null)
            {
                BadRequest(Vistas.ParametroNuloOVacio);
            }

            if (itemId == 0)
            {
                BadRequest(Vistas.ParametroNuloOVacio);
            }

            try
            {
                var msg = await Middleware.UpdateAsync(item, itemId);
                return Ok(msg);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("DeleteAsync")]
        public virtual async Task<IHttpActionResult> DeleteAsync(int itemId)
        {
            if (itemId == 0)
            {
                BadRequest(Vistas.ParametroNuloOVacio);
            }

            try
            {
                var msg = await Middleware.DeleteAsync(itemId);
                return Ok(msg);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
