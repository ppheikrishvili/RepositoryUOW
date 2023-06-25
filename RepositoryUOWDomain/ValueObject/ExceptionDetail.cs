
using System.Net;

namespace RepositoryUOWDomain.ValueObject;

public class ExceptionDetail
{
    public HttpStatusCode StatusCoDe { get; set; }

    public string? Message { get; set; }
}