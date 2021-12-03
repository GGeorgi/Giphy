namespace Giphy.Application.Threading;

public class TaskQueue
{
    public Guid Id { get; } = Guid.NewGuid();
    private readonly SemaphoreSlim semaphore;

    public TaskQueue()
    {
        semaphore = new SemaphoreSlim(1);
    }

    public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
    {
        await semaphore.WaitAsync();
        try
        {
            return await taskGenerator();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task Enqueue(Func<Task> taskGenerator)
    {
        await semaphore.WaitAsync();
        try
        {
            await taskGenerator();
        }
        finally
        {
            semaphore.Release();
        }
    }
}