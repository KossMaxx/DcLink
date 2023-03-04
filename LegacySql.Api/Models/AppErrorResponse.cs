using System;

namespace LegacySql.Api.Models
{
    public class AppErrorResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public AppErrorResponse InnerException { get; set; }

        public AppErrorResponse(Exception ex)
        {
            Type = ex.GetType().Name;
            Message = ex.Message;
            GetExceptionMessage(ex, this);
        }

        private void GetExceptionMessage(Exception e, AppErrorResponse response)
        {
            if (e.InnerException != null)
            {
                response.InnerException = new AppErrorResponse(e.InnerException);
                GetExceptionMessage(e.InnerException, InnerException);
            }
        }
    }
}
