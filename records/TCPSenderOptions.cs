namespace netlib.records;

public record TCPSenderOptions
{
    public string? Hostname { get; init; }
    public int Port { get; init; }
}