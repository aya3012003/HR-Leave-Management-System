namespace Project_3.src.Application.ExceptionHandling
{
    public class DuplicateLeaveTypeException:Exception
    {
        public DuplicateLeaveTypeException(string name) :
            base($"Leave type '{name}' already exists.") { }
    }
}
