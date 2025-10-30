namespace DocumentEditor.Interfaces;
// Helper interface: если команда влияет на ресурсы (вставка/удаление изображения),
// она должна уметь корректировать ресурсы при удалении команды из истории
interface IResourceAffecting
{
    // called when command is removed from history permanently.
    // removedFromUndone = true means this was removed while in undone stack removal (we removed an undone command)
    // removedFromUndone = false means it was an executed command removed due to overflow or removal of earliest commands.
    void OnHistoryRemoved( bool removedFromUndone );
}