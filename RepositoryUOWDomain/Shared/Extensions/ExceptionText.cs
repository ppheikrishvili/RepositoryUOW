namespace RepositoryUOWDomain.Shared.Extensions;

public static class ExceptionText
{
    public static async Task<string?> ToErrorStr(this Exception? ex, CancellationToken token = default)
    {
        string errorStr = "";
        Exception? tmpEx = ex;

        await Task.Run(() =>
        {
            while (tmpEx != null)
            {
                errorStr += $"{tmpEx.Message}{Environment.NewLine}";
                tmpEx = tmpEx.InnerException;
            }
        }, token).ConfigureAwait(false);
        return errorStr;
    }
}