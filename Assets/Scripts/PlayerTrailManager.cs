using UnityEngine;
public class PlayerTrailManager : MonoBehaviour
{
    private LightTrailGeneration ltGen;
    private void OnEnable()
    {
        ltGen = GetComponent<LightTrailGeneration>();
    }
    public void SetTrailColor(Color color)
    {
        if(color == Color.black)
        {
            // rainbow
        }
        if(ltGen == null)
        {
            ltGen = GetComponent<LightTrailGeneration>();
        }
        ltGen.SetColor(color);
    }
}
