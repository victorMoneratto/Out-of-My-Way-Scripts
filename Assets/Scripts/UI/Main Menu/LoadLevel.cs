using System;
using OMWGame.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OMWGame {
    public class LoadLevel : MonoBehaviour {

	    // TODO @OMW Replace with scene reference asset when that is implemented
	    [Required]
	    [SerializeField] private String sceneName;

	    [SerializeField] private bool asynchronous = false;
	    
	    private AsyncOperation asyncLoad = null;
	    
	    // This is a temporary workaround to clear ephemeral data
	    // from serialized scriptableobjects
	    // TODO @OMW Create a mechanism to define runset as ephemeral and
	    // that should be cleared when level is unloaded
	    [SerializeField] private GameObjectRunSet[] runSetsToClear;

	    private void Start() {
		    if (asynchronous) {
			    asyncLoad = SceneManager.LoadSceneAsync(sceneName);
			    asyncLoad.allowSceneActivation = false;
		    }
	    }

	    public void Load() {
		    if (asynchronous && asyncLoad != null) {
				asyncLoad.allowSceneActivation = true;
		    } else {
			    SceneManager.LoadScene(sceneName);
		    }

		    if (runSetsToClear != null) {
			    foreach (var runSet in runSetsToClear) {
				    runSet.Items.Clear();
			    }
		    }
	    }
    }
}