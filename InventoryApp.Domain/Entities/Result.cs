namespace InventoryApp.Domain.Entities;

public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new List<string>();

    public static Result Ok(string message = "")
    {
        return new Result { Success = true, Message = message };
    }

    public static Result Fail(string message, List<string>? errors = null)
    {
        return new Result
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Ok(T data, string message = "")
    {
        return new Result<T> { Success = true, Message = message, Data = data };
    }

    public new static Result<T> Fail(string message, List<string>? errors = null)
    {
        return new Result<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}