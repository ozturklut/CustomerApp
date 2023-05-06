namespace Web.Models.Base
{
    public class ApiBaseResponseModel<T> where T : new()
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }
    }
}
