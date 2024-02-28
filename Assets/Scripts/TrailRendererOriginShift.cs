using UnityEngine;

public class TrailRendererOriginShift : MonoBehaviour
{

    [SerializeField]
    private TrailRenderer[] TrailRenderers;

    private void OnEnable()
    {
        FloatingOrigin.OriginShiftEventChannel += OriginShift;
    }

    private void OnDisable()
    {
        FloatingOrigin.OriginShiftEventChannel -= OriginShift;
    }

    private void OriginShift(Vector3 offset)
    {
        foreach (var trailRenderer in TrailRenderers)
        {
            OriginShift(trailRenderer, offset);
        }
    }

    private void OriginShift(TrailRenderer trailRenderer, Vector3 offset)
    {
        var positions = new Vector3[trailRenderer.positionCount];
        trailRenderer.GetPositions(positions);

        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] += offset;
        }

        trailRenderer.SetPositions(positions);
    }
}
