using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

using Umbraco.Core.Logging;

namespace BackofficeTweaking.Helpers
{
    public class ExceptionHelper
    {

        public static string GetExceptionMessage(Exception ex, bool includeStackTrace = true)
        {
            StringBuilder exceptionMessage = new StringBuilder();
            exceptionMessage.AppendLine(ex.Message);
            if (ex.InnerException != null)
            {
                exceptionMessage.AppendLine(ex.InnerException.Message);
            }
            StackTrace stackTrace = new StackTrace(true);
            exceptionMessage.AppendLine(stackTrace.ToString());
            return exceptionMessage.ToString();
        }

        public static void LogExceptionMessage(Type callingType, Exception ex, bool includeStackTrace = true)
        {
            if (callingType == null)
            {
                callingType = typeof(ExceptionHelper);
            }
            LogHelper.Error(callingType, GetExceptionMessage(ex, includeStackTrace), ex);
        }

    }
}