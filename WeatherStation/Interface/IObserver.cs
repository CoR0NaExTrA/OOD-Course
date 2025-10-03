public interface IObserver<T>
{
    // Теперь Update получает данные AND источник уведомления
    void Update( T data, IObservable<T> source );
}

public interface IObservable<T>
{
    // При регистрации можно задавать priority (по умолчанию 0)
    void RegisterObserver( IObserver<T> observer, int priority = 0 );
    void RemoveObserver( IObserver<T> observer );
    void NotifyObservers();
}
