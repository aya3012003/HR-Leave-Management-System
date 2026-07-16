namespace Project_3.src.Application.ExceptionHandling
{
    public abstract class NotFoundException : Exception
    {
        protected NotFoundException(string message) : base(message) { }

    }
}
