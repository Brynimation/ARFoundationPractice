using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;



public class CollisionDetection : MonoBehaviour
{
    [SerializeField] LayerMask answerLayerMask;
    public MeshRenderer mRenderer; 
    public GameObject correctAnswerSelectedExplosion;
    public GameObject incorrectAnswerSelectedExplosion;
    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();

    }

    private void OnDrawGizmos()
    {

        if (mRenderer != null) 
        {
            //Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
        }
        
    }
    void Update()
    {
        Vector3 centreScreenSpace = Camera.main.WorldToScreenPoint(mRenderer.bounds.center);
        Ray ray = Camera.main.ScreenPointToRay(centreScreenSpace);
        RaycastHit hit;

        //Debug.Log("update");

        if (Physics.SphereCast(ray, mRenderer.bounds.extents.magnitude/2f, out hit, answerLayerMask)) 
        {
            string name = hit.collider.gameObject.name;
            string tag = hit.collider.tag;
            int scoreChange = tag == "Correct" ? 1 : -1;
            GameObject explosion = tag == "Correct" ? correctAnswerSelectedExplosion : incorrectAnswerSelectedExplosion;
            Instantiate(explosion, hit.collider.transform.position, Quaternion.Euler(90f, 0f, 0f));
            Balloon.OnDestroyBalloon?.Invoke(scoreChange);
            Destroy(hit.collider.gameObject);
        }

    }
}
