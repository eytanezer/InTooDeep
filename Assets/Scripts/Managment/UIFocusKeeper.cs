using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Managment
{
    /// <summary>
    /// keep menu options selected when pressing empty space
    /// </summary>
    public class UIFocusKeeper : MonoBehaviour
    {
        private GameObject _lastSelected;

        private void Update()
        {
            if (!EventSystem.current) return;
            
            if (EventSystem.current.currentSelectedGameObject) _lastSelected = EventSystem.current.currentSelectedGameObject;
            else if(_lastSelected && _lastSelected.activeInHierarchy) EventSystem.current.SetSelectedGameObject(_lastSelected);
        }
    }
}