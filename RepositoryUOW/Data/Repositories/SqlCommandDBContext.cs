using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using RepositoryUOWDomain.Interface;

namespace RepositoryUOW.Data.Repositories;

public class SqlCommandDBContext : ISqlCommandDBContext
{
    public async Task<DataTable> SqlCommandExecuteReaderAsync(string sqlCommandText, CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = new AppDbContext().Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using DbCommand command = connection.CreateCommand();
        command.CommandTimeout = 1360;
        command.CommandText = sqlCommandText;
        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        DataTable table = new();
        table.Load(reader);
        return table;
    }

    public async Task<int> SqlCommandExecuteCommandAsync(string sqlCommandText, CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = new AppDbContext().Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using DbCommand command = connection.CreateCommand();
        command.CommandTimeout = 1360;
        command.CommandText = sqlCommandText;
        return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}