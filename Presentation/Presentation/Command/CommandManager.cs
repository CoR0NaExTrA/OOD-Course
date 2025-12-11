using System.Collections.Generic;

namespace Presentation.Command;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class UndoRedoManager
{
    private readonly Stack<ICommand> undo = new();
    private readonly Stack<ICommand> redo = new();

    public void Execute( ICommand cmd )
    {
        cmd.Execute();
        undo.Push( cmd );
        redo.Clear();
    }

    public void Undo()
    {
        if ( undo.Count == 0 )
            return;
        var cmd = undo.Pop();
        cmd.Undo();
        redo.Push( cmd );
    }

    public void Redo()
    {
        if ( redo.Count == 0 )
            return;
        var cmd = redo.Pop();
        cmd.Execute();
        undo.Push( cmd );
    }
}
