namespace VehicleParts.Application.Common.Models;

public class ServiceResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;

    public static ServiceResult Ok(string message = "Success")
    {
        return new ServiceResult { Success = true, Message = message };
    }

    public static ServiceResult Fail(string message)
    {
        return new ServiceResult { Success = false, Message = message };
    }
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; init; }

    public static ServiceResult<T> Ok(T data, string message = "Success")
    {
        return new ServiceResult<T> { Success = true, Message = message, Data = data };
    }

    public new static ServiceResult<T> Fail(string message)
    {
        return new ServiceResult<T> { Success = false, Message = message, Data = default };
    }
}
