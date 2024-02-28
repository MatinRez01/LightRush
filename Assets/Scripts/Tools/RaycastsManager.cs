using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utilities {
    namespace RaycastsManager
    {
        public struct ray
        {
            public Vector3 origin;
            public Vector3 hitPoint;
            public Vector3 normal;
            public bool hit;
            public float distance;
        }
        public class RaycastsManager
        {
            [SerializeField] private static Color raycastsHitColor = Color.green;
            [SerializeField] private static Color raycastsUnhitColor = Color.red;
            public static bool Raycast(Vector3 start, Vector3 direction, out RaycastHit hit, float distance, bool drawgizmos, float time = 0)
            {
                var didHit = Physics.Raycast(start, direction, out hit, distance);
                if (drawgizmos)
                {
                    Debug.DrawRay(start, direction * (didHit ? hit.distance : distance), didHit ? raycastsHitColor : raycastsUnhitColor, time);
                }
                return didHit;

            }
            public static bool Raycast(Vector3 start, Vector3 direction, out RaycastHit hit, float distance, LayerMask layermask, bool drawgizmos, float time = 0)
            {
                var didHit = Physics.Raycast(start, direction, out hit, distance, layermask);
                if (drawgizmos)
                {
                    Debug.DrawRay(start, direction * (didHit ? hit.distance : distance), didHit ? raycastsHitColor : raycastsUnhitColor, time);
                    Debug.DrawRay(hit.point, hit.normal * (didHit ? hit.distance : distance), didHit ? raycastsHitColor : raycastsUnhitColor, time);

                }
                return didHit;
                
            }

            public static bool Raycast(Vector3 start, Vector3 direction, out RaycastHit hit, float distance, string tag, bool drawgizmos, float time = 0)
            {
                bool didHit;
                if (Physics.Raycast(start, direction, out hit, distance) && hit.collider.gameObject.CompareTag(tag))
                {
                    didHit = true;
                } else
                {
                    didHit = false;
                }
                if (drawgizmos)
                {
                    Debug.DrawRay(start, direction * (didHit ? hit.distance : distance), didHit ? raycastsHitColor : raycastsUnhitColor, time);
                }
                return didHit;

            }
            public static bool Raycast(Vector3 start, Vector3 direction, float distance, LayerMask layermask, bool drawgizmos, float time = 0)
            {
                RaycastHit hit;
                var didHit = Physics.Raycast(start, direction, out hit, distance, layermask);
                if (drawgizmos)
                {
                    Debug.DrawRay(start, direction * (didHit ? hit.distance : distance), didHit ? raycastsHitColor : raycastsUnhitColor, time);
                }
                return didHit;

            }
            public static bool RaycastsScattering(Vector3 origin, Vector3 endPoint, Vector3 direction, int resolution, float raysDistance, LayerMask layer, bool DrawGizmos, float time = 0f )
            {
                
                ray[] rays = new ray[resolution];
                for (int i = 0; i < resolution; i++)
                {
                    var p = Vector3.Lerp(origin, endPoint, i / (float)(resolution - 1));
                    rays[i].origin = p;
                    if (RaycastsManager.Raycast(p, direction, out RaycastHit rh, raysDistance, layer, DrawGizmos, time))
                    {
                       
                        rays[i].hit = true;
                        rays[i].hitPoint = rh.point;
                        rays[i].distance = rh.distance;
                    }
                    else
                    {
                        rays[i].hit = false;
                        rays[i].distance = raysDistance;
                    }

                }
                var hitCount = 0;
                foreach (var p in rays)
                {
                    if (p.hit)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == rays.Length)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool RaycastsScatteringRot(Vector3 origin, Vector3 direction, float angle, LayerMask layer, int resolution, float raysDistance, out RaycastHit hit, bool DrawGizmos, float time = 0f)
            {

                ray[] rays = new ray[resolution];
                Vector3 originRot = Quaternion.Euler(0, -angle, 0) * direction;
                Vector3 endRot = Quaternion.Euler(0, angle, 0) * direction;
                var hitCount = 0;
                hit = new RaycastHit();

                for (int i = 0; i < resolution; i++)
                {
                    var p = Vector3.Lerp(originRot, endRot, i / (float)(resolution - 1));
                   // rays[i]. = p;
                    if (RaycastsManager.Raycast(origin, p, out RaycastHit rh, raysDistance, layer, DrawGizmos, time))
                    {
                        rays[i].normal = rh.normal;
                        rays[i].hit = true;
                        rays[i].hitPoint = rh.point;
                        rays[i].distance = rh.distance;
                        hitCount++;
                        hit.point += rh.point;
                        hit.normal += rh.normal;
                    }
                    else
                    {
                        rays[i].hit = false;
                        rays[i].distance = raysDistance;
                    }

                }
                hit.normal /= hitCount;
                hit.point /= hitCount;
                if (hitCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool RaycastsScatteringRotOneHit(Vector3 origin, Vector3 direction, float angle, int resolution, float raysDistance,  LayerMask layer,out RaycastHit hit, bool DrawGizmos, float time = 0f)
            {
                Vector3 originRot = Quaternion.Euler(0, -angle, 0) * direction;
                Vector3 endRot = Quaternion.Euler(0, angle, 0) * direction;
                hit = new RaycastHit();
                for (int i = 0; i < resolution; i++)
                {
                    var p = Vector3.Lerp(originRot, endRot, i / (float)(resolution - 1));
                    if (RaycastsManager.Raycast(origin, p, out hit, raysDistance, layer, DrawGizmos, time))
                    {
                        return true;
                        
                    }

                }
                return false;
            }

            public static bool RaycastsScattering(Vector3 origin, Vector3 endPoint, Vector3 direction, int resolution, float raysDistance, string tag, bool DrawGizmos, float time = 0f)
            {
                ray[] rays = new ray[resolution];
                for (int i = 0; i < resolution; i++)
                {
                    var p = Vector3.Lerp(origin, endPoint, i / (float)(resolution - 1));
                    rays[i].origin = p;
                    if (RaycastsManager.Raycast(p, direction, out RaycastHit rh, raysDistance, tag, DrawGizmos, time))
                    {

                        rays[i].hit = true;
                        rays[i].hitPoint = rh.point;
                        rays[i].distance = rh.distance;
                    }
                    else
                    {
                        rays[i].hit = false;
                        rays[i].distance = raysDistance;
                    }

                }
                var hitCount = 0;
                foreach (var p in rays)
                {
                    if (p.hit)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == rays.Length)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static ray[] RaycastsScattering(Vector3 origin, Vector3 endPoint, Vector3 direction, float raysDistance, int resolution , LayerMask layer, bool DrawGizmos)
            {
                ray[] rays = new ray[resolution];
                for (int i = 0; i < resolution; i++)
                {
                    var p = Vector3.Lerp(origin, endPoint, i / (float)(resolution - 1));
                    rays[i].origin = p;
                    if (RaycastsManager.Raycast(p, direction, out RaycastHit rh, raysDistance, layer, DrawGizmos))
                    {
                        rays[i].hit = true;
                        rays[i].hitPoint = rh.point;
                        rays[i].distance = rh.distance;
                    }
                    else
                    {
                        rays[i].hit = false;
                        rays[i].distance = raysDistance;
                    }
                }
                return rays;               
            }
            public static float ComputeRaysStandardDeviation(ray[] rays)
            {
                var hitCount = 0;
                float total = 0;
                foreach (var p in rays)
                {
                    if (p.hit)
                    {
                        total += p.distance;
                        hitCount++;
                    }
                }
                float averageHeight = total / rays.Length;
                float totalVariances = 0f;
                for (int i = 0; i < rays.Length; i++)
                {
                    totalVariances += Mathf.Pow(rays[i].distance - averageHeight, 2);
                }
               float variance = Mathf.Sqrt(totalVariances / hitCount);
                return variance;
            }

        }
    }
}
