namespace Project_3.src.Application.ExceptionHandling
{
    public class DuplicateDepartmentException : ConflictException
    {
        public DuplicateDepartmentException(string name) : base($"Department with name '{name}' already exists.") { }
    }
}
