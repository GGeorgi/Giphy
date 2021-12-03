namespace Giphy.Persistance.Helpers;

public class PipelineManager
{
    private List<Task> _tasks;
    private readonly List<List<Task>> _allTasks;
    private readonly bool allowCommit;

    public PipelineManager(PipelineManager? pipeline = null)
    {
        if (pipeline == null)
        {
            _allTasks = new List<List<Task>> { new() };
            _tasks = _allTasks.First();
            allowCommit = true;
        }
        else
        {
            _tasks = pipeline._allTasks.Last();
            _allTasks = pipeline._allTasks;
            allowCommit = false;
        }
    }

    public void AddTask(Task t)
    {
        if (_tasks.Count >= 1000)
        {
            _allTasks.Add(new List<Task>());
            _tasks = _allTasks.Last();
        }

        _tasks.Add(t);
    }

    public Task CommitAsync()
    {
        return !allowCommit ? Task.CompletedTask : Task.WhenAll(_allTasks.SelectMany(x => x));
    }
}