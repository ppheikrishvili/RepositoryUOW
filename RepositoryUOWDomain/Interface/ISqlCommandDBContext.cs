
using System.Data;

namespace RepositoryUOWDomain.Interface;

public interface ISqlCommandDBContext
{
    Task<DataTable> SqlCommandExecuteReaderAsync(string sqlCommandText, CancellationToken cancellationToken = default);
    Task<int> SqlCommandExecuteCommandAsync(string sqlCommandText, CancellationToken cancellationToken = default);
}