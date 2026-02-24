namespace FusionAPI;

public enum FusionStatus
{
    Waiting,
    Done,
    Error,
    Warning
}

public enum FusionSignal
{
    Start,
    End
}

public class Result(FusionStatus status, string message)
{
    public FusionStatus Status { get; } = status;

    public string Message { get; } = message;

    /// <summary>
    /// Checks if the {Status} is the one provided
    /// </summary>
    public bool Is(FusionStatus s)
    {
        return Status == s;
    }
}