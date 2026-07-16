namespace Project_3.src.Application.ExceptionHandling
{
    public class EmployeeLeaveBalanceNotFoundException : NotFoundException
    {
        public EmployeeLeaveBalanceNotFoundException(int id ) : base($"Employee leave balance with id {id} not found.")
        { }
    
    }
}
