namespace Subway.Mvp.Application.Health;

/// <summary>
/// HealthQueue - Queue with limit - action removes item
/// Pass in an Action to event Item when popped off the list
/// Usages: 
///     1) When item is popped process item -> process item
///     2) When item is popped -> process remaining items -> calculations
/// </summary>
/// <typeparam name="TClass"></typeparam>
internal sealed class HealthQueue<TClass> : IDisposable where TClass : class
{
    /// <summary>
    /// Action for last item removed
    /// </summary>
    private Action<TClass> _lastItemRemoved;

    private HealthQueue()
    {
    }

    public HealthQueue(int capacity, Action<TClass> lastItemRemoved)
    {
        Capacity = capacity > 0 ? capacity : throw new ArgumentException($"{nameof(capacity)} is greater than 0.");
        _lastItemRemoved = lastItemRemoved ?? throw new ArgumentNullException(nameof(lastItemRemoved));
        CanAutoPop = true;
    }

    public HealthQueue(int capacity, Action<TClass> lastItemRemoved, IEnumerable<TClass> collection)
        : this(capacity, lastItemRemoved)
    {
        if (collection != null && collection.Any())
        {
            Parallel.ForEach(collection, Push);
        }
    }

    /// <summary>
    /// ~HealthQueue
    /// </summary>
    ~HealthQueue()
    {
        Dispose(false);
    }

    /// <summary>
    /// is auto pop enabled
    /// </summary>
    private bool CanAutoPop
    {
        get;
        set;
    }

    /// <summary>
    /// list of {TClass}
    /// </summary>
    private Queue<TClass> Items
    {
        get;
    } = new Queue<TClass>();

    /// <summary>
    /// retrieve the last item from the stack
    /// </summary>
    public TClass LastItem => Items != null && Items.Count > 0 ? Items.Peek() : default;

    /// <summary>
    /// Count
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Capacity
    /// </summary>
    public int Capacity
    {
        get;
        private set;
    }

    /// <summary>
    /// Push
    /// </summary>
    /// <param name="value"></param>
    public void Push(TClass value)
    {
        Items.Enqueue(value);

        if (CanAutoPop && Count > Capacity)
        {
            AutoPop();
        }
    }

    /// <summary>
    /// GetAll
    /// </summary>
    public IEnumerable<TClass> GetAll()
    {
        if (Items.Count > 0)
        {
            return Items.Select(x => x);
        }
        return [];
    }

    /// <summary>
    /// AutoPop
    /// </summary>
    private void AutoPop()
    {
        if (_lastItemRemoved != default)
        {
            Task.Run(() => Interlocked.CompareExchange(
                    ref _lastItemRemoved!, null, null)?.Invoke(Items.Dequeue()));
        }
    }

    /// <summary>
    /// Pop
    /// </summary>
    public TClass Pop()
    {
        if (Items.Peek() != default)
        {
            return Items.Dequeue();
        }
        return default;
    }    

    /// <summary>
    /// Disposed
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// clear items in the list
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing)
    {

        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Items.Clear();

            _disposed = true;
        }
    }
}
