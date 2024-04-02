using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextAndImage
{
    public Sprite image;

    [TextArea]
    public string text, title;
}
