using UnityEngine;

public class HoverScript : MonoBehaviour
{
    public void StartHover(Texture2D hover_cursor){
        Cursor.SetCursor(hover_cursor,Vector2.zero,CursorMode.Auto);
    }

    public void EndHover(){
        Cursor.SetCursor(null,Vector2.zero,CursorMode.Auto);
    }
}
