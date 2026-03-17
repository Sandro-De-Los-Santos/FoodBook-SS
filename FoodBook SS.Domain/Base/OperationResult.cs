namespace FoodBook_SS.Domain.Base
{
    public class OperationResult
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public static OperationResult Ok(object? data = null, string message = "")
            => new() { Success = true, Data = data, Message = message };
        public static OperationResult Fail(string message)
            => new() { Success = false, Message = message };
    }
}
