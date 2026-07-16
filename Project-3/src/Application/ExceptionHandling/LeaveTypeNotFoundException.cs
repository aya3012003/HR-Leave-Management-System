namespace Project_3.src.Application.ExceptionHandling
{
    public class LeaveTypeNotFoundException:Exception
    {
        public LeaveTypeNotFoundException(int  id ) :
           base($"Leave type '{id}' not found.")
        { }
    }
}
