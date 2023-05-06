using Data.Enums;
using Data.Models.Error;
using Service.Extensions;

namespace Service.Helpers
{
    public static class ExceptionHelper
    {
        public static void ThrowIf(this bool condition, string message)
        {
            if (condition)
            {
                Error exception = new(ErrorCode: ErrorCode.WrongParamaeters, message);
                throw exception;
            }
        }

        public static void ThrowIfIsNullOrNegative(this int? i)
        {
            if (i.IsNullOrNegative())
            {
                Error exception = new(ErrorCode: ErrorCode.WrongParamaeters, $"Parameter cannot be null or negative {nameof(i)}");
                throw exception;
            }
        }

        public static void ThrowIfAny(this int[]? param)
        {
            if (!(param?.Any() ?? false))
            {
                Error exception = new(ErrorCode: ErrorCode.WrongParamaeters, $"Parameter cannot be null {nameof(param)}");
                throw exception;
            }
        }

        public static void ThrowIfAny(this List<int>? param)
        {
            if (!(param?.Any() ?? false))
            {
                Error exception = new(ErrorCode: ErrorCode.WrongParamaeters, $"Parameter cannot be null {nameof(param)}");
                throw exception;
            }
        }

        public static void ThrowIfEmpty(this string? param)
        {
            if (param == null || param == "")
            {
                Error exception = new(ErrorCode: ErrorCode.WrongParamaeters, $"Parameter cannot be null or empty {nameof(param)}");
                throw exception;
            }
        }
    }
}
