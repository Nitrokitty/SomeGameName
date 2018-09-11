using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAbilities : MonoBehaviour {


    public float drawbackSpeed = 1f;
    public float sliceSpeed = 1f;
    public float defenseSpeed = 10f;

    Vector3 restingPosition;
    Quaternion restingRotation;
    public Vector3 drawbackRotationDifference = new Vector3(90, -90, -40);
    public Vector3 drawbackPositionDifference = new Vector3(-.5f, 0, .5f);
    Quaternion drawbackRotation;
    Vector3 drawbackPosition;
    Vector3 slicePosition;
    Quaternion sliceRotation;
    Vector3 defensePosition;
    Quaternion defenseRotation;

    GameObject hilt;
    GameObject tip;

    float startTime;
    float currentTime;
    float journeyLength;
    float distanceCovered;
    float currentJourneyFraction;
    SwordState state;
    bool previousShift;
    
    public SwordState State
    {
        get { return state; }
    }

    public float Damage
    {
        get { return 10; }
    }

    // Use this for initialization
    void Start () {
        restingRotation = transform.localRotation;
        restingPosition = transform.localPosition;

        drawbackRotation = Quaternion.Euler(restingRotation.eulerAngles - drawbackRotationDifference);
        drawbackPosition = restingPosition - drawbackPositionDifference;

        slicePosition = new Vector3(-drawbackPosition.x, drawbackPosition.y, drawbackPosition.z);
        sliceRotation = Quaternion.Euler(new Vector3(drawbackRotation.eulerAngles.x, 90, drawbackRotation.eulerAngles.z));

        defensePosition = new Vector3(0, restingPosition.y, restingPosition.z);
        defenseRotation = Quaternion.Euler(new Vector3(restingRotation.x, -180, -90));

        hilt = transform.Find("Hilt").gameObject;
        tip = transform.Find("Tip").gameObject;
        state = SwordState.Resting;
        previousShift = false;
    }
	
	// Update is called once per frame
	void Update () {
        var slice = Input.GetKey("1");

        if ((slice && state == SwordState.Resting) || state == SwordState.DrawingBack)
        {
            DrawBack();
            if (currentJourneyFraction > .99f)
                Slice();
        } else if(state == SwordState.Slice)
        {
            Slice();
            if (currentJourneyFraction > .99f)
                ReturnSlice();
        }
        else if (state == SwordState.ReturnSlice)
        {
            ReturnSlice();
            if (currentJourneyFraction > .99f)
                if(slice)
                    Slice();
                else
                    ReturnDrawBack();
        } else if (state == SwordState.ReturnDrawBack)
        {
            ReturnDrawBack();
            if (currentJourneyFraction > .99f)
                state = SwordState.Resting;
        }

        var currentShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if ((previousShift && !currentShift) || state == SwordState.ReturnDefending)
        {
            ReturnDefending();
            if (currentJourneyFraction > .99f)
                state = SwordState.Resting;
        }

        else if (currentShift)
        {           
            if (state == SwordState.Defending && currentJourneyFraction > .99f)
            {                
                return;
            }
            Defending();

        }
              
        previousShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
    }

    void Slice() {
       
        if (state != SwordState.Slice)
        {
            startTime = Time.time;
            journeyLength = Vector3.Distance(drawbackPosition, slicePosition);
            state = SwordState.Slice;
        }
        MoveSwordSlerp(drawbackPosition, slicePosition, drawbackRotation, sliceRotation, sliceSpeed);
    }

    void ReturnSlice() {
        if (state != SwordState.ReturnSlice)
        {
            startTime = Time.time;
            journeyLength = Vector3.Distance(slicePosition, drawbackPosition);
            state = SwordState.ReturnSlice;
        }
        MoveSwordSlerp(slicePosition, drawbackPosition, sliceRotation, drawbackRotation, sliceSpeed);
    }

    void DrawBack() {
        if(state != SwordState.DrawingBack)
        {
            startTime = Time.time;   
            journeyLength = Vector3.Distance(restingPosition, drawbackPosition);           
            state = SwordState.DrawingBack;
        }
        MoveSwordLerp(restingPosition, drawbackPosition, restingRotation, drawbackRotation, drawbackSpeed);
    }

    void ReturnDrawBack()
    {
        if (state != SwordState.ReturnDrawBack)
        {
            startTime = Time.time;
            journeyLength = Vector3.Distance(drawbackPosition, restingPosition);
            state = SwordState.ReturnDrawBack;
        }
        MoveSwordLerp(drawbackPosition, restingPosition, drawbackRotation, restingRotation, drawbackSpeed);
    }

    void Defending()
    {
        if (state != SwordState.Defending)
        {
            startTime = Time.time;
            journeyLength = Vector3.Distance(restingPosition, defensePosition);
            state = SwordState.Defending;
        }
        
        MoveSwordLerp(restingPosition, defensePosition, restingRotation, defenseRotation, defenseSpeed);
    }

    void ReturnDefending()
    {
        if (state != SwordState.ReturnDefending)
        {
            startTime = Time.time;
            journeyLength = Vector3.Distance(defensePosition, restingPosition);
            state = SwordState.ReturnDefending;
        }

        MoveSwordLerp(defensePosition, restingPosition, defenseRotation, restingRotation, defenseSpeed);
    }

    void MoveSwordLerp(Vector3 start, Vector3 end, Quaternion startRotation, Quaternion endRotation, float speed)
    {
        distanceCovered = (Time.time - startTime) * speed;
        currentJourneyFraction = distanceCovered / journeyLength;

        transform.localPosition = Vector3.Lerp(start, end, currentJourneyFraction);
        transform.localRotation = Quaternion.Lerp(startRotation, endRotation, currentJourneyFraction);
    }

    void MoveSwordSlerp(Vector3 start, Vector3 end, Quaternion startRotation, Quaternion endRotation, float speed)
    {
        distanceCovered = (Time.time - startTime) * speed;
        currentJourneyFraction = distanceCovered / journeyLength;

        transform.localPosition = Vector3.Slerp(start, end, currentJourneyFraction);
        transform.localRotation = Quaternion.Slerp(startRotation, endRotation, currentJourneyFraction);
    }

    public enum SwordState
    {
        Resting,
        DrawingBack,
        Slice,
        ReturnSlice,
        ReturnDrawBack,
        Defending,
        ReturnDefending
    }

}
