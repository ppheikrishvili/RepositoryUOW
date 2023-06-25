using RepositoryUOWDomain.Interface;
using RepositoryUOWDomain.Shared.Enums;

namespace RepositoryUOW.Data.Exceptions;

public class RequestException : Exception, IRequestException
{
    public ResponseCodeEnum ResponseCode { get; set; }

    public RequestException() => ResponseCode = ResponseCodeEnum.Success;

    public RequestException(string textMessage, ResponseCodeEnum responseCode) : base(textMessage) => ResponseCode = responseCode;

    public RequestException(ResponseCodeEnum responseCode, Exception ex) : base(ex.Message, ex.InnerException) => ResponseCode = responseCode;

    public RequestException(Exception ex) : base(ex.Message, ex.InnerException)
    {
    }

    public RequestException(string errorText, Exception ex) : base($"{errorText}{Environment.NewLine}{ex.Message}", ex.InnerException)
    {
    }
}