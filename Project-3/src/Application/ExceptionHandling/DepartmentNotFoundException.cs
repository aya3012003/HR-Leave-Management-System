namespace Project_3.src.Application.ExceptionHandling
{
    public class DepartmentNotFoundException: NotFoundException
    {
        public DepartmentNotFoundException(int id) : base($"Department with id {id} not found.") { }
    }
}
