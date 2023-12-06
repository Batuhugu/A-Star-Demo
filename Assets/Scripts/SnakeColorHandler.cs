using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnakeColorHandler : MonoBehaviour
{
    [Header("Snake Color Ingame")]
    [SerializeField] GameObject segmentPrefab;
    [SerializeField] GameObject snakeHead;

    [Header("Snake Color Preview")]
    [SerializeField] Image previewHead;
    [SerializeField] List<Image> previewBody;

    
    public void SetColor(Color headColor, Color bodyColor)
    {
        // Changing color of the ingame snake.
        snakeHead.GetComponent<SpriteRenderer>().color = headColor;
        segmentPrefab.GetComponent<SpriteRenderer>().color = bodyColor;

        // Changing color of the preview snake.
        previewHead.color = headColor;
        if (previewBody != null && previewBody.Count > 0)
        {
            foreach (Image image in previewBody)
            {
                image.color = bodyColor;
            }
        }
    }
}
