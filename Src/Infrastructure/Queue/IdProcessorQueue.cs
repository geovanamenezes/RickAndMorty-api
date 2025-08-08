using System.Threading.Channels;

public static class IdProcessorQueue
{
    public static Channel<string> Queue { get; } = Channel.CreateUnbounded<string>();
}
