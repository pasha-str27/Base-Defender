using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FireVirtualButton : Button
{
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        var targetColor =
            state == SelectionState.Disabled ? colors.disabledColor :
            state == SelectionState.Highlighted ? colors.highlightedColor :
            state == SelectionState.Normal ? colors.normalColor :
            state == SelectionState.Pressed ? colors.pressedColor :
            state == SelectionState.Selected ? colors.selectedColor : Color.white;

        targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
        transform.parent.GetComponent<Button>().targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
    }
}
