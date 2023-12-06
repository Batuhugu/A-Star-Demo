using UnityEngine;
using UnityEngine.UI;

public class SnakeColor : MonoBehaviour
{
    [SerializeField] Image headdColor;
    [SerializeField] Image bodyyColor;
    [SerializeField] SnakeColorHandler colorHandlerScript;

    public void ChangeColor()
    {
        colorHandlerScript.SetColor(headdColor.color, bodyyColor.color);
    }

}
