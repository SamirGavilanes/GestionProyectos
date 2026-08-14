namespace GestionProyectos.Shared.Message
{
    public static class OperationResultExtensions
    {
        public static TResult GetDataOrDefault<TResult>(this OperationResult<TResult> result, TResult fallback)
            => result.Data ?? fallback;
    }
}
