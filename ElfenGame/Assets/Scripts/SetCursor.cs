using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    public Texture2D glove;
    // Start is called before the first frame update
    void Start()
    {
        Vector2 cursorOffset = new Vector2(0, 0);

        Cursor.SetCursor(glove, cursorOffset, CursorMode.Auto);
    }

}
