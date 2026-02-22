using UnityEngine;

[ExecuteInEditMode]
public class FPSManagerScript : MonoBehaviour
{
    [SerializeField] private int frameRate;
    void Awake()
    {
        Application.targetFrameRate = 60; // Set the target frame rate to 60 FPS
        #if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
        #endif
    }
}
