using RepositoryUOWDomain.Shared.Enums;

namespace RepositoryUOWDomain.Interface;

public interface IResponseResult<T>
{
    string? ResponseStr { get; set; }

    T ReturnValue { get; set; }

    ResponseCodeEnum ResponseCode { get; set; }

    bool IsServiceError { get; set; }
}