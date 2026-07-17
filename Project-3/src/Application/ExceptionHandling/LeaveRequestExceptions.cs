namespace Project_3.src.Application.ExceptionHandling
{
    public class LeaveRequestNotFoundException : NotFoundException
    {
        public LeaveRequestNotFoundException(int id) : base($"Leave request with id {id} not found.") { }
    }

    public class InsufficientLeaveBalanceException : ConflictException
    {
        public InsufficientLeaveBalanceException() : base("Employee has insufficient leave balance for this request.") { }
    }
}
