using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class HumanBodyTracker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The skeleton prefab to be controlled")]
    GameObject skeletonPrefab;

    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce body tracking events")]
    ARHumanBodyManager humanBodyManager;

    //Getter/setter for the ARHumanBodyManager
    public ARHumanBodyManager HumanBodyManager 
    {
        get { return humanBodyManager;}
        set { humanBodyManager = value;}
    }

    //Getter/setter for the skeleton prefab
    public GameObject SkeletonPrefab 
    {
        get { return skeletonPrefab;}
        set { skeletonPrefab = value; }
    }
    //A TrackableId uniquely identifies a trackable (like a plane or feature point) in a given session.
    Dictionary<TrackableId, BoneController> skeletonTracker = new Dictionary<TrackableId, BoneController>();

    private void OnEnable()
    {
        humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }
    private void OnDisable()
    {
        humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }
    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs) 
    {
        BoneController boneController;

        foreach (ARHumanBody humanBody in eventArgs.added) 
        {
            //If the current bodypart is not already being tracked
            if (!skeletonTracker.TryGetValue(humanBody.trackableId, out boneController)) 
            {
                GameObject newSkeleton = Instantiate(skeletonPrefab, humanBody.transform);
                boneController = newSkeleton.GetComponent<BoneController>();
                skeletonTracker.Add(humanBody.trackableId, boneController);
            }
            boneController.InitializeSkeletonJoints();
            boneController.ApplyBodyPose(humanBody);
        }

        foreach (ARHumanBody humanBody in eventArgs.updated) 
        {
            if (skeletonTracker.TryGetValue(humanBody.trackableId, out boneController)) 
            {
                boneController.ApplyBodyPose(humanBody);
            }
        }
        foreach (ARHumanBody humanBody in eventArgs.removed)
        {
            if (skeletonTracker.TryGetValue(humanBody.trackableId, out boneController)) 
            {
                Destroy(boneController.gameObject);
                skeletonTracker.Remove(humanBody.trackableId);
            }
        }
        
    } 

    // Update is called once per frame
    void Update()
    {
        
    }
}
