namespace Project_3.src.Application.ExceptionHandling
{
    public class DuplicateEmployeeLeaveBalanceException : ConflictException
    {
        public DuplicateEmployeeLeaveBalanceException() : base($"Employee already has a leave balance for this leave type.")
        { }
        public DuplicateEmployeeLeaveBalanceException(int employeeId, int leaveTypeId) :
            base($"Employee with id {employeeId} already has a leave balance for leave type with id {leaveTypeId}.")
        { }
    
    }
}
