namespace Project_3.src.Application.ExceptionHandling
{
    public class DuplicateLeaveTypeException: ConflictException
    {
        public DuplicateLeaveTypeException(string name) :
            base($"Leave type '{name}' already exists.") { }
    }
}
