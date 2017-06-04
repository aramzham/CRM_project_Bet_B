using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace CRM.WebApi.Infrastructure
{
    public class NotImplExceptionAttribute : ExceptionFilterAttribute
    {
        private readonly LoggerManager _logger = new LoggerManager();
        public override Task OnExceptionAsync(HttpActionExecutedContext action, CancellationToken cancellationToken)
        {
            _logger.LogError(action.Exception, action.Request.Method, action.Request.RequestUri);
            // null reference error
            if (action.Exception is ArgumentNullException)
                action.Response = new HttpResponseMessage
                {
                    Content = new StringContent(string.Format($"Argument exception handled.\n{action.Exception.Message}\n{action.Exception.InnerException?.Message}")),
                    StatusCode = HttpStatusCode.BadRequest
                };
            else if (action.Exception is NullReferenceException)
            {
                action.Response = new HttpResponseMessage
                {
                    Content = new StringContent(string.Format($"Null reference exception.\n{action.Exception.Message}\n{action.Exception.InnerException?.Message}")),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            else if (action.Exception is NoNullAllowedException)
            {
                action.Response = new HttpResponseMessage
                {
                    Content = new StringContent(string.Format($"Null exception\n{action.Exception.Message}\n{action.Exception.InnerException?.Message}")),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            // data exception
            else if (action.Exception is DataException)
                action.Response = new HttpResponseMessage
                {
                    Content = new StringContent(string.Format($"Data exception.\n{action.Exception.Message}\n{action.Exception.InnerException?.Message}")),
                    StatusCode = HttpStatusCode.Conflict
                };
            // entity exception
            else if (action.Exception is EntityException)
                action.Response = new HttpResponseMessage
                {
                    Content = new StringContent(string.Format($"Entity exception.\n{action.Exception.Message}\n{action.Exception.InnerException?.Message}")),
                    StatusCode = HttpStatusCode.Conflict
                };
            // default case
            else
                action.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            return base.OnExceptionAsync(action, cancellationToken);
        }
    }
}