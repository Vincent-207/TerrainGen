// using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public RectTransform compassBar, northMarker, southMark, objectiveMarker;
    public Transform cameraObjTransform, objectiveObj;
    public static Compass instance;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    void Update()
    {
        if(objectiveObj != null) SetMarkerPosition(objectiveMarker, objectiveObj.position);
        SetMarkerPosition(northMarker, Vector3.forward * 1000f);
        SetMarkerPosition(southMark, Vector3.back * 1000f);
    }

    void SetMarkerPosition(RectTransform markerTransform, Vector3 worldPos)
    {
        Vector3 toTarget = worldPos - cameraObjTransform.position;

        float angle = Vector2.SignedAngle(new Vector2(toTarget.x, toTarget.z), new Vector2(cameraObjTransform.forward.x, cameraObjTransform.forward.z)); 
        float compassPosX = Mathf.Clamp(2 * angle / Camera.main.fieldOfView, -1, 1);
        markerTransform.anchoredPosition = new Vector2(compassBar.rect.width/2 * compassPosX, 0);
    }

    public void UpdateObjectiveTransform(Transform objectiveTransform)
    {
        objectiveObj = objectiveTransform;
    }
}
