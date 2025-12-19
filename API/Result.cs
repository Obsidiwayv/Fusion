namespace Fusion.API;

public enum AtomicStatus
{
    ERROR = 0x0001,
    DONE = 0x0002,
    WAITING = 0x0004
}

/// <seealso cref="GenericAtomicResult{T}"/>
/// <summary>
/// Creates a result based class for waiting or erroring using an
/// <see cref="AtomicStatus"/>
/// </summary>
public class AtomicResult(AtomicStatus status, string? message)
{
    public AtomicStatus Status { get; set; } = status;

    public string? Message { get; set; } = message;

    /// <summary>
    /// Returns a readonly and stripped down version of <strong>AtomicResult</strong>
    /// to prevent overwriting of the result
    /// </summary>
    /// <returns>ReadonlyAtomicResult</returns>
    public ReadonlyAtomicResult Lock()
    {
        return new ReadonlyAtomicResult(Status, Message);
    }

    public void Invalidate(string msg)
    {
        Status = AtomicStatus.ERROR;
        Message = msg;
    }
    
    public static bool IsErrored(AtomicStatus st) {
        return st == AtomicStatus.ERROR;
    }
}

public class ReadonlyAtomicResult(AtomicStatus status, string? message)
{
    public AtomicStatus Status { get; } = status;

    public string? Message { get; } = message;
}

/// <inheritdoc cref="AtomicResult"/>
/// <typeparam name="T">
/// Item type that will be used for 
/// <see cref="GenericAtomicResult{T}.EmitItem(Action{T})"/>
/// </typeparam>
public class GenericAtomicResult<T>(AtomicStatus status, T? item)
{

    public AtomicStatus Status { get; set; } = status;

    /// <summary>
    /// Emit the item if not errored or item is not null
    /// </summary>
    public void EmitItem(Action<T> func)
    {
        if (!AtomicResult.IsErrored(Status) && item != null)
        {
            func.Invoke(item);
        }
    }


}