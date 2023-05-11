using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net;
using System;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.XR.ARFoundation;
/*public class Balloon : MonoBehaviour
{
public Vector2 viewportPosition;
public Material mat;
public float distanceFromNearClipPlane;

Collider2D col;
MeshFilter mf;
MeshRenderer mRenderer;
GameObject balloonMesh;

void Awake() 
{
col = GetComponent<Collider2D>();
Mesh mesh = col.CreateMesh(true, true);
mf = GetComponent<MeshFilter>();
balloonMesh = new GameObject("Balloon Mesh");
balloonMesh.AddComponent<MeshFilter>().mesh = mesh;
balloonMesh.AddComponent<MeshRenderer>().material = mat;
balloonMesh.AddComponent<MeshCollider>();
}

void Update()
{
Vector3 viewport = new Vector3(viewportPosition.x, viewportPosition.y, Camera.main.nearClipPlane + distanceFromNearClipPlane);
balloonMesh.transform.position = Camera.main.ViewportToWorldPoint(viewport);


}
}*/
public class Balloon : MonoBehaviour
{
    public Vector2 viewportPosition;
    public static Action<int> OnDestroyBalloon;
    public Canvas worldCanvas;
    public TMP_Text answerText;
    public float distanceFromNearClipPlane;
    public Renderer mRenderer;
    public Renderer faceRenderer;
    public GameObject correctParticleExplosion;
    public GameObject incorrectParticleExplosion;

    ARFace ARFace;
    GameObject explosionToInstantiate;
    Collider col;
    private void Awake()
    {
        mRenderer = GetComponent<Renderer>();
#if UNITY_EDITOR
        faceRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>();
#endif
        worldCanvas = GetComponentInChildren<Canvas>();
        col = GetComponent<Collider>();

#if !UNITY_EDITOR
        Vector3 localScale = answerText.transform.localScale;
        localScale.x *= -1;
        answerText.transform.localScale = localScale;
        StartCoroutine(TryGetFace());
#endif
    }


    IEnumerator TryGetFace()
    {
        ARFace = FindObjectOfType<ARFace>();
        while (ARFace == null)
        {
            ARFace = FindObjectOfType<ARFace>();
            yield return null;
        }
        faceRenderer = ARFace.gameObject.GetComponent<Renderer>();
    }

    private void OnDrawGizmos()
    {
        if (mRenderer == null || faceRenderer == null) return;
        Gizmos.DrawWireCube(mRenderer.bounds.center, mRenderer.bounds.extents * 2);
        Gizmos.DrawWireCube(faceRenderer.bounds.center, faceRenderer.bounds.extents * 2);
    }

    static bool CheckScreenSpaceCollision(Bounds bounds1, Bounds bounds2)
    {
        Vector3 min1 = Camera.main.WorldToScreenPoint(bounds1.min);
        Vector3 max1 = Camera.main.WorldToScreenPoint(bounds1.max);

        Vector3 min2 = Camera.main.WorldToScreenPoint(bounds2.min);
        Vector3 max2 = Camera.main.WorldToScreenPoint(bounds2.max);

        
        return !(min1.x > max2.x || min2.x > max1.x || min1.y > max2.y || min2.y > max1.y);
    }

    static bool CheckScreenSpaceCollision2(Bounds bounds1, Bounds bounds2)
    {

        Vector3 centre1 = Camera.main.WorldToScreenPoint(bounds1.center);
        centre1.z = 0;
        Vector3 centre2 = Camera.main.WorldToScreenPoint(bounds2.center);
        centre2.z = 0;
        Vector3 min1 = Camera.main.WorldToScreenPoint(bounds1.min);
        min1.z = 0;
        Vector3 min2 = Camera.main.WorldToScreenPoint(bounds2.min);
        min2.z = 0;
        float screenSpaceRadius1 = Vector3.Distance(centre1, min1);
        float screenSpaceRadius2 = Vector3.Distance(centre2, min2);
        //Debug.Log(screenSpaceRadius1);
        //Debug.Log(screenSpaceRadius2);

        
        float distance = Vector3.Distance(centre2, centre1);

        return distance <= (screenSpaceRadius1 + screenSpaceRadius2);
    }

    static bool CheckScreenSpaceCollision3(Bounds bounds1, Bounds bounds2)
    {

        Vector3 max1 = Camera.main.WorldToScreenPoint(bounds1.max);
        max1.z = 0;
        Vector3 max2 = Camera.main.WorldToScreenPoint(bounds2.max);
        max2.z = 0;
        Vector3 min1 = Camera.main.WorldToScreenPoint(bounds1.min);
        min1.z = 0;
        Vector3 min2 = Camera.main.WorldToScreenPoint(bounds2.min);
        min2.z = 0;
        float screenSpaceRadius1 = Vector3.Distance(max1, min1)/2f;
        float screenSpaceRadius2 = Vector3.Distance(max2, min2)/2f;


        Vector3 centre1 = Camera.main.WorldToScreenPoint(bounds1.center);
        max1.z = 0;
        Vector3 centre2 = Camera.main.WorldToScreenPoint(bounds2.center);
        max2.z = 0;

        float distance = Vector2.Distance(centre2, centre1);

        return distance <= (screenSpaceRadius1 + screenSpaceRadius2);
    }

    public void InitialiseAnswer(Vector2 viewportPos, string answer, int curIndex, List<int> correctIndices) 
    {
        viewportPosition = viewportPos;
        answerText.SetText(answer);
        gameObject.tag = (correctIndices.Contains(curIndex)) ? "Correct" : "Incorrect";
    }
    void Update2()
    {
        Vector3 viewport = new Vector3(viewportPosition.x, viewportPosition.y, Camera.main.nearClipPlane + distanceFromNearClipPlane);
        transform.position = Camera.main.ViewportToWorldPoint(viewport);

    }

    void Update()
    {
        Vector3 viewport = new Vector3(viewportPosition.x, viewportPosition.y, Camera.main.nearClipPlane + distanceFromNearClipPlane);
        transform.position = Camera.main.ViewportToWorldPoint(viewport);
        
        if (faceRenderer != null) 
        {
            
            if (CheckScreenSpaceCollision(mRenderer.bounds, faceRenderer.bounds))
            {
                int scoreChange = tag == "Correct" ? 1 : -1;
                //OnDestroyBalloon?.Invoke(scoreChange);
                //Destroy(this.gameObject);
            }
        }
        else
        {
            if (ARFace == null) ARFace = FindObjectOfType<ARFace>();
            if (ARFace != null)
            {
                faceRenderer = ARFace.gameObject.GetComponent<Renderer>();
            }

        }

    }

}
