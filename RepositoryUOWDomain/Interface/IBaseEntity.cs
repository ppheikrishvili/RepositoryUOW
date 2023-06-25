
namespace RepositoryUOWDomain.Interface;

public interface IBaseEntity
{
    bool IsValid();
    Task<bool> IsValidAsync();
    string? AttributeError { get; set; }
    //int Id { get; set; }
}