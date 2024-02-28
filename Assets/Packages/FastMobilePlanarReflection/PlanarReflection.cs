using UnityEngine;

public class PlanarReflection : MonoBehaviour
{
    public bool VR = false;
    public int ReflectionTexResolution = 512;
    public float Offset = 0.0f;
    [Range(0, 1)]
    public float ReflectionAlpha=0.5f;
    public bool BlurredReflection;
    public LayerMask LayersToReflect = -1;

    private Camera reflectionCamera;
    private RenderTexture reflectionTexture = null, reflectionTextureRight = null;
    private static bool isRendering = false;
    private Material material;
    private Camera cam;
    private static readonly int reflectionTexString = Shader.PropertyToID("_ReflectionTex");
    private static readonly int reflectionTexRString = Shader.PropertyToID("_ReflectionTexRight");
    private static readonly int reflectionAlphaString = Shader.PropertyToID("_RefAlpha");
    private static readonly string blurString = "BLUR";
    private static readonly string vrString = "VRon";
    private Matrix4x4 reflectionMatrix;
    private Vector4 reflectionPlane;
    private Vector3 posistion;
    private Vector3 normal;
    private Vector4 oblique;
    private Matrix4x4 worldToCameraMatrix;
    private Vector3 clipNormal;
    private Vector4 clipPlane;
    private Vector3 oldPosition;
    Vector3 eulerAngles;

    public void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        material = GetComponent<Renderer>().sharedMaterials[0];
        QualitySettings.pixelLightCount = 0;

        GameObject go = new GameObject(GetInstanceID().ToString(), typeof(Camera), typeof(Skybox));
        reflectionCamera = go.GetComponent<Camera>();
        reflectionCamera.enabled = false;
        reflectionCamera.transform.position = transform.position;
        reflectionCamera.transform.rotation = transform.rotation;
        reflectionCamera.gameObject.AddComponent<FlareLayer>();
        reflectionCamera.cullingMask = ~(1 << 4) & LayersToReflect.value;
        reflectionCamera.cameraType = CameraType.Reflection;

        go.hideFlags = HideFlags.HideAndDontSave;

        if (reflectionTexture)
        {
            DestroyImmediate(reflectionTexture);
        }
        reflectionTexture = new RenderTexture(ReflectionTexResolution, ReflectionTexResolution, 16);
        reflectionTexture.isPowerOfTwo = true;
        reflectionTexture.hideFlags = HideFlags.DontSave;

        if (reflectionTextureRight)
        {
            DestroyImmediate(reflectionTextureRight);
        }
        reflectionTextureRight = new RenderTexture(ReflectionTexResolution, ReflectionTexResolution, 16);
        reflectionTextureRight.isPowerOfTwo = true;
        reflectionTextureRight.hideFlags = HideFlags.DontSave;

        if (cam.clearFlags == CameraClearFlags.Skybox)
        {
            Skybox sky = cam.GetComponent(typeof(Skybox)) as Skybox;
            Skybox mysky = reflectionCamera.GetComponent(typeof(Skybox)) as Skybox;
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }
    }
    public void OnWillRenderObject()
    {
        if (isRendering)
        {
            return;
        }

        isRendering = true;
        posistion = transform.position;
        normal = transform.up;

        reflectionCamera.clearFlags = cam.clearFlags;
        reflectionCamera.backgroundColor = cam.backgroundColor;
        reflectionCamera.farClipPlane = cam.farClipPlane;
        reflectionCamera.nearClipPlane = cam.nearClipPlane;
        reflectionCamera.orthographic = cam.orthographic;
        reflectionCamera.fieldOfView = cam.fieldOfView;
        reflectionCamera.aspect = cam.aspect;
        reflectionCamera.orthographicSize = cam.orthographicSize;

        reflectionPlane = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, posistion) - Offset);

        reflectionMatrix.m00 = (1F - 2F * reflectionPlane[0] * reflectionPlane[0]);
        reflectionMatrix.m01 = (-2F * reflectionPlane[0] * reflectionPlane[1]);
        reflectionMatrix.m02 = (-2F * reflectionPlane[0] * reflectionPlane[2]);
        reflectionMatrix.m03 = (-2F * reflectionPlane[3] * reflectionPlane[0]);
        reflectionMatrix.m10 = (-2F * reflectionPlane[1] * reflectionPlane[0]);
        reflectionMatrix.m11 = (1F - 2F * reflectionPlane[1] * reflectionPlane[1]);
        reflectionMatrix.m12 = (-2F * reflectionPlane[1] * reflectionPlane[2]);
        reflectionMatrix.m13 = (-2F * reflectionPlane[3] * reflectionPlane[1]);
        reflectionMatrix.m20 = (-2F * reflectionPlane[2] * reflectionPlane[0]);
        reflectionMatrix.m21 = (-2F * reflectionPlane[2] * reflectionPlane[1]);
        reflectionMatrix.m22 = (1F - 2F * reflectionPlane[2] * reflectionPlane[2]);
        reflectionMatrix.m23 = (-2F * reflectionPlane[3] * reflectionPlane[2]);
        reflectionMatrix.m30 = 0F;
        reflectionMatrix.m31 = 0F;
        reflectionMatrix.m32 = 0F;
        reflectionMatrix.m33 = 1F;

        oldPosition = cam.transform.position;
        reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflectionMatrix;

        worldToCameraMatrix = reflectionCamera.worldToCameraMatrix;
        clipNormal = worldToCameraMatrix.MultiplyVector(normal).normalized;
        clipPlane = new Vector4(clipNormal.x, clipNormal.y, clipNormal.z, -Vector3.Dot(worldToCameraMatrix.MultiplyPoint(posistion + normal * Offset), clipNormal));

        if (!VR)
        {
            RenderObjectCamera(cam.projectionMatrix, false);
            material.DisableKeyword(vrString);
        }
        else
        {
            RenderObjectCamera(cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left), false);
            RenderObjectCamera(cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right), true);
            material.EnableKeyword(vrString);
        }

        material.SetTexture(reflectionTexString, reflectionTexture);
        material.SetFloat(reflectionAlphaString, ReflectionAlpha);

        if (VR)
        {
            material.SetTexture(reflectionTexRString, reflectionTextureRight);
        }


        if (BlurredReflection)
        {
            material.EnableKeyword(blurString);
        }
        else
        {
            material.DisableKeyword(blurString);
        }

        isRendering = false;
    }

    void OnDisable()
    {
        if (reflectionTexture)
        {
            DestroyImmediate(reflectionTexture);
            reflectionTexture = null;
        }
        if (reflectionTextureRight)
        {
            DestroyImmediate(reflectionTextureRight);
            reflectionTextureRight = null;
        }
        if (reflectionCamera)
        {
            DestroyImmediate(reflectionCamera.gameObject);
            reflectionCamera = null;
        }
    }

    private void RenderObjectCamera(Matrix4x4 projection, bool right)
    {
        oblique = clipPlane * (2.0F / (Vector4.Dot(clipPlane, projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1.0f, 1.0f))));
        projection[2] = oblique.x - projection[3];
        projection[6] = oblique.y - projection[7];
        projection[10] = oblique.z - projection[11];
        projection[14] = oblique.w - projection[15];
        reflectionCamera.projectionMatrix = projection;
        reflectionCamera.targetTexture = right ? reflectionTextureRight : reflectionTexture;

        GL.invertCulling = true;
        reflectionCamera.transform.position = reflectionMatrix.MultiplyPoint(oldPosition);
        eulerAngles = cam.transform.eulerAngles;
        reflectionCamera.transform.eulerAngles = new Vector3(0, eulerAngles.y, eulerAngles.z);
        reflectionCamera.Render();
        reflectionCamera.transform.position = oldPosition;
        GL.invertCulling = false;
    }

    private static float sgn(float a) => a > 0.0f ? 1.0f : a < 0.0f ? -1.0f : 0.0f;
}
