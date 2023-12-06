using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonStateHandler : MonoBehaviour
{
    [SerializeField] List<Button> buttonList;

    [SerializeField] float alphaReductionPercentage = 0.5f;

    public void CheckScore(int score)
    {
        // Make buttons interactable/uninteractable according to score.
        for (int i = 1; i < buttonList.Count; i++)
        {
            // Check if the score is higher than the threshold for this button
            if (score > i * 10)
            {
                buttonList[i].interactable = true;
            }
            else
            {
                buttonList[i].interactable = false;
            }
        }
    }

    public void SetObjectsAlpha()
    {
        foreach (Button button in buttonList)
        {
            if (!button.interactable)
            {
                foreach (Graphic childObject in button.GetComponentsInChildren<Graphic>())
                {
                    Color childColor = childObject.color;
                    childColor.a *= (1 - alphaReductionPercentage);
                    childObject.color = childColor;
                }
            }
            else
            {
                foreach (Graphic childObject in button.GetComponentsInChildren<Graphic>())
                {
                    Color childColor = childObject.color;
                    childColor.a = 1;
                    childObject.color = childColor;
                }
            }
        }
    }

}
