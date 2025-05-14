using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SwipeLasso : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform swipeArea;
    public Material ropeMaterial;
    public Material torusMaterial;
    public float maxSwipeDistance = 8000f;
    public float maxLassoDistance = 6f;
    public float lassoSpeed = 10f;
    public float lassoArcHeight = 1.5f;

    public Transform emptyParent;
    public RuntimeAnimatorController anim1;
    public Vector3 torusPos;
    public bool orbCaptured = false;
    public bool cubeCaptured = false;
    private Vector2 swipeStart;
    private Vector2 swipeEnd;
    private bool swiping = false;




    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("HI");
        if (RectTransformUtility.RectangleContainsScreenPoint(swipeArea, eventData.position))
        {
            swiping = true;
            swipeStart = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("YO");
        if (swiping)
        {
            swipeEnd = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("BYE");
        if (!swiping) return;

        swipeEnd = eventData.position;
        swiping = false;

        Vector2 swipeVector = swipeEnd - swipeStart;
        float swipeLength = Mathf.Min(swipeVector.magnitude, maxSwipeDistance);
        Vector2 swipeDirection = swipeVector.normalized;

        // Screen midpoint as the origin for throwing
        Vector2 screenBottom = new Vector2(Screen.width / 2f, 0);
        Vector2 screenTarget = screenBottom + swipeDirection * swipeLength;

        // Project into world space at a fixed distance from camera
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(new Vector3(screenTarget.x, screenTarget.y, maxLassoDistance));

        FireLasso(targetPos);
    }

    public void CustomPointerDown(BaseEventData data)
    {
        OnPointerDown((PointerEventData)data);
    }

    public void CustomPointerUp(BaseEventData data)
    {
        OnPointerUp((PointerEventData)data);
    }

    public void CustomDrag(BaseEventData data)
    {
        OnDrag((PointerEventData)data);
    }


    private void FireLasso(Vector3 target)
    {
        Vector3 origin = Camera.main.transform.position + new Vector3(0, -0.5f, -0.5f); // near bottom of view

        // Target positions for raycasts slightly above target final height
        Vector3 targetAtMedHeight = new Vector3(target.x, target.y + .5f, target.z);
        Vector3 targetAtMaxHeight = new Vector3(target.x, target.y + 1.25f, target.z);
        float totalDistance = Vector3.Distance(origin, target);

        RaycastHit hit;
        // If raycast hits a collider, lasso goes to object location instead of initial target position
        if (Physics.Raycast(origin, targetAtMaxHeight, out hit, totalDistance) || Physics.Raycast(origin, targetAtMedHeight, out hit, totalDistance))
        {
            print(hit.collider.gameObject.name);
            StartCoroutine(AnimateLasso(origin, hit.collider.gameObject.transform.position, hit.collider.gameObject.name));
        }
        // Else goes to initial target position
        else
        {
            StartCoroutine(AnimateLasso(origin, target, "none"));
        }
    }

    private IEnumerator AnimateLasso(Vector3 origin, Vector3 target, string lassoedObjectName)
    {
        // Create rope
        GameObject ropeObj = new GameObject("LassoRope");
        LineRenderer line = ropeObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.material = ropeMaterial;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;

        // Create torus
        GameObject torus = CreateTorus();
        torus.transform.position = origin;
        torus.name = "LassoHook";

        float totalDistance = Vector3.Distance(origin, target);
        float outgoingSpeed = lassoSpeed * 1.5f;  // Speed up outgoing
        float returnSpeed = lassoSpeed * 2.0f;     // Even faster return

        float outgoingTravelTime = totalDistance / outgoingSpeed;
        float returnTravelTime = totalDistance / returnSpeed;

        
        // 1. Outgoing (with arc)
        float timer = 0f;
        while (timer < outgoingTravelTime)
        {
            float t = timer / outgoingTravelTime;
            t = Mathf.Clamp01(t);

            Vector3 pos = Vector3.Lerp(origin, target, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * lassoArcHeight;

            torus.transform.position = pos;

            torusPos = new Vector3(torus.transform.position.x, torus.transform.position.y - 0.2f, torus.transform.position.z + 0.2f);

            // If object got lassoed, finds object in dictionary and sets "is being lassoed" to true
            if (lassoedObjectName != "none") {
                foreach (var kvp in GameManager.modelDictionary)
                {
                    GameObject modelObject = kvp.Key;
                    DataModelInfoSO modelInfo = kvp.Value;
                    
                    if(modelInfo.name == lassoedObjectName)
                    {
                        modelInfo.isBeingLassoed = true;
                    }
                }
            }

            line.SetPosition(0, origin);
            line.SetPosition(1, pos);

            timer += Time.deltaTime;
            yield return null;
        }

        // Snap to final position
        torus.transform.position = target;
        line.SetPosition(1, target);

        // 2. Wait at maximum extension
        yield return new WaitForSeconds(1.0f); // <- wait 2 seconds

        // 3. Returning (no arc)
        timer = 0f;
        while (timer < returnTravelTime)
        {
            float t = timer / returnTravelTime;
            t = Mathf.Clamp01(t);

            Vector3 pos = Vector3.Lerp(target, origin, t);

            torus.transform.position = pos;

            // Sets lassoed object position to follow torus position as it returns
            foreach (var kvp in GameManager.modelDictionary)
            {
                GameObject modelObject = kvp.Key;
                DataModelInfoSO modelInfo = kvp.Value;
                
                if(modelInfo.isBeingLassoed == true)
                {
                    modelObject.transform.position = torus.transform.position;
                }
            }

            line.SetPosition(0, origin);
            line.SetPosition(1, pos);

            timer += Time.deltaTime;
            yield return null;
        }

        // Snap to final position
        torus.transform.position = origin;
        line.SetPosition(1, origin);

        // Cleanup
        Destroy(ropeObj);
        Destroy(torus);

        // Updates variables for lassoed objects and sets inactive in game scene
        foreach (var kvp in GameManager.modelDictionary)
        {
            GameObject modelObject = kvp.Key;
            DataModelInfoSO modelInfo = kvp.Value;
                
            if(modelInfo.isBeingLassoed == true)
            {
                modelInfo.isBeingLassoed = false;
                modelInfo.isCaptured = true;
                modelObject.SetActive(false);
            }
        }
    }


    private GameObject CreateTorus(int segments = 30, int tubeSegments = 15, float radius = 0.4f, float tubeRadius = 0.03f)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(segments + 1) * (tubeSegments + 1)];
        int[] triangles = new int[segments * tubeSegments * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        int vert = 0, tri = 0;
        for (int i = 0; i <= segments; i++)
        {
            float theta = (float)i / segments * 2f * Mathf.PI;
            Vector3 circleCenter = new Vector3(Mathf.Cos(theta) * radius, 0, Mathf.Sin(theta) * radius);

            for (int j = 0; j <= tubeSegments; j++)
            {
                float phi = (float)j / tubeSegments * 2f * Mathf.PI;
                Vector3 normal = new Vector3(Mathf.Cos(phi) * Mathf.Cos(theta), Mathf.Sin(phi), Mathf.Cos(phi) * Mathf.Sin(theta));
                vertices[vert] = circleCenter + normal * tubeRadius;
                uv[vert] = new Vector2((float)i / segments, (float)j / tubeSegments);
                vert++;
            }
        }

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < tubeSegments; j++)
            {
                int current = i * (tubeSegments + 1) + j;
                int next = current + tubeSegments + 1;

                triangles[tri++] = current;
                triangles[tri++] = next;
                triangles[tri++] = current + 1;

                triangles[tri++] = current + 1;
                triangles[tri++] = next;
                triangles[tri++] = next + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        GameObject torusObj = new GameObject("Torus");
        MeshFilter filter = torusObj.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer renderer = torusObj.AddComponent<MeshRenderer>();
        renderer.material = torusMaterial;

        return torusObj;
    }
}
