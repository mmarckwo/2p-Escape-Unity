using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SceneLoad : NetworkBehaviour
{
    public string SceneRef;

    private void OnTriggerEnter(Collider other)
    {
        if (Runner.SceneManager.IsReady(Runner)) Debug.Log("scene is ready");
        Runner.SetActiveScene(SceneRef);
        
    }
}
