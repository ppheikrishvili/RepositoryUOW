using System.Net;

namespace RepositoryUOWDomain.ValueObject;

public class ExceptionDetail
{
    public HttpStatusCode StatusCode { get; set; }

    public string? Message { get; set; }
}