using DocumentEditor.Commands;
using DocumentEditor.Interfaces;

namespace DocumentEditor.Model;

// Отдельный класс для управления историей команд
public class CommandHistory
{
    private readonly List<ICommand> _history = new();
    private readonly Stack<ICommand> _undone = new();
    private const int MaxHistory = 10;

    private readonly Document _doc;

    public CommandHistory( Document doc )
    {
        _doc = doc;
    }

    public bool CanUndo() => _history.Count > 0;
    public bool CanRedo() => _undone.Count > 0;

    public void ExecuteCommand( ICommand cmd )
    {
        // Очистка undone-стека (если после Undo выполняется новая команда)
        if ( _undone.Count > 0 )
        {
            while ( _undone.Count > 0 )
            {
                var undone = _undone.Pop();
                if ( undone is IResourceAffecting ra )
                    ra.OnHistoryRemoved( removedFromUndone: true );
            }
        }

        // Попробовать склеить команды
        var last = _history.LastOrDefault();
        if ( last != null && last.CanMergeWith( cmd ) )
        {
            last.MergeWith( cmd );
            return;
        }

        // Выполнение и добавление в историю
        cmd.Execute();
        _history.Add( cmd );

        // Ограничение длины истории
        TrimOldest();
    }

    public void Undo()
    {
        if ( !CanUndo() )
        {
            Console.WriteLine( "Nothing to undo" );
            return;
        }

        var cmd = _history.Last();
        _history.RemoveAt( _history.Count - 1 );
        cmd.Unexecute();
        _undone.Push( cmd );
    }

    public void Redo()
    {
        if ( !CanRedo() )
        {
            Console.WriteLine( "Nothing to redo" );
            return;
        }

        var cmd = _undone.Pop();
        cmd.Execute();
        _history.Add( cmd );
        TrimOldest();
    }

    private void TrimOldest()
    {
        while ( _history.Count > MaxHistory )
        {
            var removed = _history[ 0 ];
            _history.RemoveAt( 0 );
            if ( removed is IResourceAffecting ra )
                ra.OnHistoryRemoved( removedFromUndone: false );
        }
    }
}
