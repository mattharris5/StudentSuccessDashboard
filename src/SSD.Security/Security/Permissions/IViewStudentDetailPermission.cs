
namespace SSD.Security.Permissions
{
    public interface IViewStudentDetailPermission : IPermission
    {
        bool CustomFieldOnly { get; }
    }
}
