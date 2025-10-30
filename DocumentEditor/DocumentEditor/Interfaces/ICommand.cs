namespace DocumentEditor.Interfaces;
// Интерфейс команды (Command pattern)
interface ICommand
{
    void Execute();
    void Unexecute();
    // Для склейки последовательных команд:
    bool CanMergeWith( ICommand other );
    void MergeWith( ICommand other ); // выполняется если CanMergeWith == true
    string Describe();
    // Для управления ресурсами: возвращает список путей ресурсов (абсолютных) которые добавляет / удаляет при исполнении команды
    // Тут - для упрощения: команда может сообщить относительный путь внутри рабочей директории (images/...). Для логики удаления/восстановления достаточно булевых флагов и ссылок.
}
