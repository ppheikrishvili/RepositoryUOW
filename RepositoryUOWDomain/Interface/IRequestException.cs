using RepositoryUOWDomain.Shared.Enums;

namespace RepositoryUOWDomain.Interface;

public interface IRequestException
{
    ResponseCodeEnum ResponseCode { get; set; }
}