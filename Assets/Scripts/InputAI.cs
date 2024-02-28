using UnityEngine;

public class InputAI : InputManager
{
    private Vector3 directionToGo;
    private Vector3 cross;
    Transform player;

    private void Start()
    { 
        player = GameObject.FindWithTag("Player").transform;
    }
    public override float GetSteerDirection()
    {
        directionToGo = (player.position - transform.position).normalized;
        cross = Vector3.Cross(transform.forward, directionToGo);
        return cross.y;
    }
}
