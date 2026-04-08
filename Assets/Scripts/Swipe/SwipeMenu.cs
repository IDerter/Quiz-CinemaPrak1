using UnityEngine;

namespace QuizCinema
{
    // Этот скрипт больше не используется. Вся логика свайпа перенесена в ScrollRectSnap.cs
    // и управляется через SwipeImage.cs.
    // Мы оставляем файл, чтобы избежать ошибок "Missing Script" в Unity,
    // но его содержимое можно безопасно очистить.
    public class SwipeMenu : SingletonBase<SwipeMenu>
    {
        // public void ResetAllSwipeArguments()
        // {
        //     // Логика сброса теперь будет обрабатываться в ScrollRectSnap.ResetToStart()
        // }
    }
}
