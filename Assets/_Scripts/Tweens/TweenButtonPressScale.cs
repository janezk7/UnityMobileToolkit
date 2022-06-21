using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tweens
{
    [RequireComponent(typeof(Button))]
    public class TweenButtonPressScale : MonoBehaviour
    {
        void Start()
        {
            var button = GetComponent<UnityEngine.UI.Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<UnityEngine.UI.Button>();
                button.transition = UnityEngine.UI.Selectable.Transition.None;
            }
            button.onClick.AddListener(() => TweenUtil.TweenPressScale(gameObject));
        }
    }
}
