using RepositoryUOWDomain.Shared.Enums;
using RepositoryUOWDomain.Interface;
using System.Text.Json.Serialization;

namespace RepositoryUOWDomain.ValueObject;

//JsonSerializerOptions options = new()
//{
//    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
//};

//string forecastJson =
//    JsonSerializer.Serialize<Forecast>(forecast, options);

public class ResponseResult<T> : IResponseResult<T>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ResponseStr { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T ReturnValue { get; set; }

    public ResponseCodeEnum ResponseCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsServiceError
    {
        get =>
            !string.IsNullOrWhiteSpace(ResponseStr) && (ResponseCode != ResponseCodeEnum.Notification) &&
            (ResponseCode != ResponseCodeEnum.Success);
        set { }
    }

    public ResponseResult(T rValue) =>
        (ReturnValue, ResponseStr, ResponseCode) = (rValue, "", ResponseCodeEnum.Success);

    public ResponseResult(string? eStr, T rValue) => (ReturnValue, ResponseStr) = (rValue, eStr);

    public ResponseResult(string? eStr, ResponseCodeEnum respCode, T rValue) =>
        (ReturnValue, ResponseStr, ResponseCode) = (rValue, eStr, respCode);

    public ResponseResult() => (ResponseStr, ReturnValue) = ("", default!);

    public static ResponseResult<ICollection<TT>> ErrorResponseResultList<TT>(string eStr, ResponseCodeEnum respCode) =>
        new(eStr, respCode, Activator.CreateInstance<List<TT>>());

    public static ResponseResult<TT> ErrorResponseResultValue<TT>(string eStr, ResponseCodeEnum respCode) =>
        new(eStr, respCode, Activator.CreateInstance<TT>());
}