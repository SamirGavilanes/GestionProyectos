namespace GestionProyectos.Shared.Message
{
    public class OperationResult<TResult>
    {
        public bool Success { get; set; }
        public TResult? Data { get; set; }
        public Message Message { get; set; } = new();
        public Exception? Exception { get; set; }
        public static OperationResult<TResult> CreateSuccessResult(TResult result)
        {
            return new OperationResult<TResult> { Success = true, Message = new Message { Code = "0", Description = "Exitoso" }, Data = result };
        }
        public static OperationResult<TResult> CreateFailureResult(string nonSuccessMessage)
        {
            return new OperationResult<TResult>
            {
                Success = false,
                Message = new Message
                {
                    Code = "1",
                    Description = nonSuccessMessage
                }
            };
        }
        public static OperationResult<TResult> CreateFailureResult(Message message)
        {
            return new OperationResult<TResult>
            {
                Success = false,
                Message = message
            };
        }
        public static OperationResult<TResult> CreateFailureResult(Exception ex)
        {
            return new OperationResult<TResult>
            {
                Success = false,
                Message = new Message
                {
                    Code = "1",
                    Description = ex.Message
                }
            };
        }
    }

    public class Message
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
