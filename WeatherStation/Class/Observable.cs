namespace WeatherStation.Core;

// Базовая реализация Observable с поддержкой приоритетов и безопасного уведомления
public abstract class Observable<T> : IObservable<T>
{
    // SortedDictionary хранит группы по приоритету.
    // Ключи отсортированы по возрастанию, будем перебирать в Reverse() для уведомления
    private readonly SortedDictionary<int, HashSet<IObserver<T>>> _groups =
        new SortedDictionary<int, HashSet<IObserver<T>>>();

    // Быстрая проверка, в какой группе (priority) зарегистрирован наблюдатель
    private readonly Dictionary<IObserver<T>, int> _observerPriorities =
        new Dictionary<IObserver<T>, int>();

    public void RegisterObserver( IObserver<T> observer, int priority = 0 )
    {
        if ( observer == null )
            throw new ArgumentNullException( nameof( observer ) );

        // Если уже подписан — игнорируем повторную подписку
        if ( _observerPriorities.ContainsKey( observer ) )
            return;

        // Получаем/создаём множество для этого приоритета
        if ( !_groups.TryGetValue( priority, out var set ) )
        {
            set = new HashSet<IObserver<T>>();
            _groups.Add( priority, set );
        }

        set.Add( observer );
        _observerPriorities[ observer ] = priority;
    }

    public void RemoveObserver( IObserver<T> observer )
    {
        if ( observer == null )
            throw new ArgumentNullException( nameof( observer ) );

        if ( !_observerPriorities.TryGetValue( observer, out var priority ) )
            return; // не было подписки

        if ( _groups.TryGetValue( priority, out var set ) )
        {
            set.Remove( observer );
            if ( set.Count == 0 )
                _groups.Remove( priority );
        }

        _observerPriorities.Remove( observer );
    }

    public void NotifyObservers()
    {
        // Получаем данные один раз
        T data = GetChangedData();

        // Делаем снимок наблюдателей в порядке убывания приоритета
        var snapshot = new List<IObserver<T>>();
        foreach ( var kv in _groups.Reverse() )
        {
            // Итерация по HashSet в произвольном порядке — допустимо
            foreach ( var obs in kv.Value )
                snapshot.Add( obs );
        }

        // Запускаем уведомления по снимку — безопасно, если кто-то удалит себя внутри Update.
        foreach ( var obs in snapshot )
        {
            try
            {
                obs.Update( data, this );
            }
            catch
            {
                // Для базовой гарантии безопасности исключений — ловим, логируем/игнорируем,
                // чтобы другие наблюдатели всё равно получили уведомление.
                // В учебной задаче достаточно заглушить исключение.
            }
        }
    }

    // Наследники обязаны вернуть текущее состояние изменений
    protected abstract T GetChangedData();
}
