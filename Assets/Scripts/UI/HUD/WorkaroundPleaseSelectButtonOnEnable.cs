using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OMWGame {
    public class WorkaroundPleaseSelectButtonOnEnable : MonoBehaviour {
        private void OnEnable() {
            StartCoroutine(SelectNullThenWaitAFrameThenSelectTheButtonsPleaseWork());
        }

        private IEnumerator SelectNullThenWaitAFrameThenSelectTheButtonsPleaseWork() {
            EventSystem.current.SetSelectedGameObject(null);
            yield return null;
            EventSystem.current.SetSelectedGameObject(gameObject);
            // yeah, it worked. mixed feelings.
        }
    }
}