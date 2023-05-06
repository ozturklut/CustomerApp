using Data.Enums;
using System.Reflection;

namespace Data.Models.Base
{
    public class ApiBaseResponseModel<T> where T : new()
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }

        public ApiBaseResponseModel()
        {

        }

        public void setError(Exception ex, MethodBase? method)
        {
            string methodName = "Unknown Method";
            Error.Error tempError = ex is Error.Error error ? error : new(ErrorCode.GeneralError, ex.Message, methodName);
            tempError.Key ??= methodName;

            Success = false;
            Error = tempError.Message;
        }

        public void setCustomError(string errorMessage, MethodBase? method = null)
        {
            string methodName = method == null ? "Unknow method" : method.Name.ToString();
            Success = false;
            Error = errorMessage;
        }
    }
}
