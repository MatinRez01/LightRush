using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using MyBox;

public class LightTrailGeneration : MonoBehaviour
{
    [SerializeField] bool player;
    //The number of vertices to create per frame
    public  int NUM_VERTICES = 12;

 
     
    [SerializeField]
    [Tooltip("The empty game object located at the tip of the blade")]
    private GameObject _tip = null;

    [SerializeField]
    [Tooltip("The empty game object located at the base of the blade")]
    private GameObject _base = null;

    [SerializeField]
    private GameObject _backLight;



    [FormerlySerializedAs("_trailFrameLength")]
    [SerializeField]
    [Tooltip("The number of frame that the trail should be rendered for")]
    private int trailFrameLength = 3;

    [SerializeField] private float validDistForVis = 1;
    [FormerlySerializedAs("_emissionColor")]
    [SerializeField]
    [ColorUsage(true, true)]
    [Tooltip("The colour of the blade and trail")]
    private Color emissionColor = Color.red;
    [SerializeField]
    private Material tempMaterial;

    [SerializeField]
    private string layerName, tagName;
    [SerializeField]
    private bool infiniteCharge;

    private bool active;


    private GameObject meshObject = null;
    private Vector3 previousTipposition;
    private Vector3 previousBaseposition;
    private LightTrail lt;
    private float trailCharge;
    private bool shouldGenerateMesh
    {
        get
        {
            return HasCharge && Active;
        }
    }

    public bool HasCharge
    {
        get
        {
            if (infiniteCharge)
            {
                return true;
            }
            else
            {
                return trailCharge >= 0f;
            }
        }
    } 
    public bool FullCharge => trailCharge > 0.95f;
    public float TrailCharge
    {
        get
        {
            return trailCharge;
        }
    }
    public bool Active
    {
        get => active;
    }

    void Start()
    {
        InitLightTrailGeneration();
        ChargeLightTrail();
    }
    private void InitLightTrailGeneration()
    {
        meshObject = new GameObject("LightTrail");
        Mesh mesh = new Mesh();

        meshObject.AddComponent<MeshFilter>().mesh = mesh;
        Material trailMaterial = Instantiate(tempMaterial);
        meshObject.AddComponent<MeshRenderer>().material = trailMaterial;
        meshObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", emissionColor);
        MeshCollider meshCollider = meshObject.AddComponent<MeshCollider>();
        int myLayer = LayerMask.NameToLayer(layerName);
        meshObject.layer = myLayer;
        meshObject.tag = tagName;
        lt = meshObject.AddComponent<LightTrail>();
        lt.Setup(mesh, meshCollider, NUM_VERTICES, trailFrameLength, validDistForVis);

        //Set starting position for tip and base
        previousTipposition = _tip.transform.position;
        previousBaseposition = _base.transform.position;
    }
    
    void LateUpdate()
    {
        if (!Active) return;
        if (HasCharge)
        {
            GenerateLightTrail();
        }
        else
        {
            if (player)
            {
                GameEvents.TriggerEvent("LightTrailChargeState", 0);
            }
            SwitchTrailState();
        }
    }

    public void SwitchTrailState()
    {
        if(!Active && !HasCharge) return;
        previousTipposition = _tip.transform.position;
        previousBaseposition = _base.transform.position;
        active = !active;
        _backLight.SetActive(Active);
        if (player)
        {
            GameEvents.TriggerEvent("LightTrailSwitch", Active ? 1 : 0);

        }
        /*       Array.Clear(vertices, 0, vertices.Length);
               Array.Clear(uvs, 0, uvs.Length);
               Array.Clear(triangles, 0, uvs.Length);*/

    }

    private void GenerateLightTrail()
    {
        trailCharge -= Time.deltaTime / 2;
/*        if (Time.frameCount % 2 != 0) return;
*/        lt.UpdateMeshValues(_base.transform.position, previousBaseposition,
            _tip.transform.position, previousTipposition);
        //Track the previous base and tip positions for the next frame
        previousTipposition = _tip.transform.position;
        previousBaseposition = _base.transform.position;
    }
    public void ChargeLightTrail()
    {
        trailCharge = 1;
        if (player)
        {
            GameEvents.TriggerEvent("LightTrailChargeState", 1);
        }

    }
    public void ChargeLightTrail(float val)
    {
        trailCharge = val;
    }

}
