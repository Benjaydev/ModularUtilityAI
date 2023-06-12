using UnityEngine;

public class UIScript : MonoBehaviour
{
    public void SetShowDebug(bool showDebug)
    {
        AITest.instances.ForEach(ai => ai.GetComponent<UtilityAI_AITest>().showDebug = showDebug);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
