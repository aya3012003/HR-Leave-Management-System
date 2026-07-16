namespace Project_3.src.Application.ExceptionHandling
{
    public abstract class ConflictException : Exception
    {
        protected ConflictException(string message) : base(message) { }
    }
}
