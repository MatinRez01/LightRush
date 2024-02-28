using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LightTrail : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 previousStartPos;
    private Vector3 previousEndPos;
    private int frameCount;
    private int verticesCount;
    private int frameLength;
    private float validDistForVis;

    public float disableVisCounter;
    private MeshRenderer meshRenderer;

    private bool meshValid
    {
        get
        {
            return mesh.bounds.size.magnitude > validDistForVis;
        }
    }
    public bool valid;
    public void Setup(Mesh mesh, MeshCollider collider, int verticesCount, int frameLength, float distance)
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        vertices = new Vector3[verticesCount * frameLength];
        triangles = new int[verticesCount * frameLength];
        uvs = new Vector2[verticesCount * frameLength];
        meshCollider = collider;
        this.mesh = mesh;
        this.frameLength = frameLength;
        this.verticesCount = verticesCount;
        validDistForVis = distance;
        disableVisCounter = this.frameLength * this.verticesCount;
    }
    // Update is called once per frame
    public void UpdateMeshValues(Vector3 currentBasePos, Vector3 previousBasePos, Vector3 currentTipPos, Vector3 previousTipPos)
    {
        if (!meshRenderer.enabled)
        {
            meshRenderer.enabled = true;
        }
        disableVisCounter = frameLength * verticesCount;
        startPos = currentBasePos;
        endPos = currentTipPos;
        previousStartPos = previousBasePos;
        previousEndPos = previousTipPos;
    }
    private void LateUpdate()
    {
        valid = meshValid;
        GenerateLightTrail();
        if (meshValid)
        {
            UpdateCollider();
            disableVisCounter -= verticesCount;

        }
        else
        {
            DisableCollider();
        }

    }

    private void GenerateLightTrail()
    {
        if(!meshValid)
        {
            previousStartPos = startPos;
            previousEndPos = endPos;
            meshRenderer.enabled = false;
        }
/*        if (Time.frameCount % 2 != 0) return;
*/        //Reset the frame count one we reach the frame length
        if (frameCount == (frameLength * verticesCount))
        {
            frameCount = 0;
        }
        //Draw first triangle vertices for back and front


        /*vertices[frameCount + 3] = (_base.transform.position);
        uvs[frameCount + 3] = Vector2.zero;
        vertices[frameCount + 4] = (previousTipposition);
        uvs[frameCount + 4] = Vector2.up;
        vertices[frameCount + 5] = (_tip.transform.position);
        uvs[frameCount + 5] = Vector2.up;*/
        //Draw fill in triangle vertices

        /*    vertices[frameCount + 6] = (previousTipposition);
            uvs[frameCount + 6] = Vector2.up;
            vertices[frameCount + 7] = (_base.transform.position);
            uvs[frameCount + 7] = Vector2.zero;
            vertices[frameCount + 8] = (previousBaseposition);
            uvs[frameCount + 8] = Vector2.zero;*/
        

        vertices[frameCount] = (startPos);
        uvs[frameCount] = Vector2.zero;
        vertices[frameCount + 1] = (endPos);
        uvs[frameCount + 1] = Vector2.up;
        vertices[frameCount + 2] = (previousEndPos);
        uvs[frameCount + 2] = Vector2.up;
        vertices[frameCount + 3] = (previousEndPos);
        uvs[frameCount + 3] = Vector2.up;
        vertices[frameCount + 4] = (previousStartPos);
        uvs[frameCount + 4] = Vector2.zero;
        vertices[frameCount + 5] = (startPos);
        uvs[frameCount + 5] = Vector2.zero;

        //Set triangles

        for (int i = 0; i < frameLength * verticesCount; i++)
        {
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        //Track the previous base and tip positions for the next frame
        frameCount += verticesCount;
    }
    private void UpdateCollider()
    {
        if (meshCollider.enabled)
        {
            meshCollider.sharedMesh = mesh;
        }
        else
        {
            meshCollider.enabled = true;
            meshCollider.sharedMesh = mesh;
        }
    }
    private void DisableCollider()
    {
        if (meshCollider.enabled)
        {
            meshCollider.enabled = false;
        }
    }

}
