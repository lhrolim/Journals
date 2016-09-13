using System.Web.Http.Filters;
using log4net;

namespace Journals.Web.Api.Filters {

    public class GenericExceptionFilter : ExceptionFilterAttribute {

        private static readonly ILog Log = LogManager.GetLogger(typeof(GenericExceptionFilter));

        public override void OnException(HttpActionExecutedContext context) {
            var e = context.Exception;
            Log.Error(e);
            throw e;
        }


    }
}