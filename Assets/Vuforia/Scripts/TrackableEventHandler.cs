/*==============================================================================
Copyright (c) 2019 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class TrackableEventHandler : MonoBehaviour, ITrackableEventHandler 
{
    #region PROTECTED_MEMBER_VARIABLES 

    protected TrackableBehaviour mTrackableBehaviour;
    protected TrackableBehaviour.Status m_PreviousStatus;
    protected TrackableBehaviour.Status m_NewStatus;

    //globale klasse
    private GameObject managerObject;
    private Manager manager;
    private string mood; // es gibt die moods = piece, building, info

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        //initialisieren des GameManagers
        managerObject = GameObject.Find("GameManager");
        manager = managerObject.GetComponent<Manager>();
        mood = "piece";

        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;
        
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + 
                  " " + mTrackableBehaviour.CurrentStatus +
                  " -- " + mTrackableBehaviour.CurrentStatusInfo);

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED /*||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED*/)
        {
            OnTrackingFound();
        }
        else if ((previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE) || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound()
    {
        if (mTrackableBehaviour)
        {

            //hinzufügen des Objektes wenn es getracked wird 
            manager.addTrackedImageToList(mTrackableBehaviour.TrackableName, gameObject.tag, gameObject);


            var rendererComponents = mTrackableBehaviour.GetComponentsInChildren<Renderer>(true);
            var colliderComponents = mTrackableBehaviour.GetComponentsInChildren<Collider>(true);
            var canvasComponents = mTrackableBehaviour.GetComponentsInChildren<Canvas>(true);

           
            // Enable rendering:
            foreach (var component in rendererComponents)
            {
                if (mood.Equals("building") && component.gameObject.tag.Equals("Building"))
                {
                    component.enabled = true;
                } else if (mood.Equals("piece") && component.gameObject.tag.Equals("Piece"))
                {
                    component.enabled = true;
                } else if (mood.Equals("info") && component.gameObject.tag.Equals("Info"))
                {
                    component.enabled = true;
                }
                
            }

            // Enable colliders:
            foreach (var component in colliderComponents)
            {
                if (mood.Equals("building") && component.gameObject.tag.Equals("Building"))
                {
                    component.enabled = true;
                }
                else if (mood.Equals("piece") && component.gameObject.tag.Equals("Piece"))
                {
                    component.enabled = true;
                }
                else if (mood.Equals("info") && component.gameObject.tag.Equals("Info"))
                {
                    component.enabled = true;
                }
            }

            // Enable canvas':
            foreach (var component in canvasComponents)
            {
                component.enabled = true;
            }
        }
    }


    protected virtual void OnTrackingLost()
    {
        if (mTrackableBehaviour)
        {
            //entfernen des OBjektes wenn es nicht mehr getracked wird
            manager.removeTrackedImageToList(mTrackableBehaviour.TrackableName, gameObject.tag);

            var rendererComponents = mTrackableBehaviour.GetComponentsInChildren<Renderer>(true);
            var colliderComponents = mTrackableBehaviour.GetComponentsInChildren<Collider>(true);
            var canvasComponents = mTrackableBehaviour.GetComponentsInChildren<Canvas>(true);

            // Disable rendering:
            foreach (var component in rendererComponents)
                component.enabled = false;

            // Disable colliders:
            foreach (var component in colliderComponents)
                component.enabled = false;

            // Disable canvas':
            foreach (var component in canvasComponents)
                component.enabled = false;
        }
    }

    #endregion // PROTECTED_METHODS

    //ändert den status was angezeigt werden soll
    public void updateMood(string mood)
    {
        Debug.Log("Der MOOD des objekts ----" + gameObject.name + "----  wird mit dem mood ----" + mood + "---- geupdated");
        this.mood = mood;
        updateRendering();
    }

    // Updated das rendering
    private void updateRendering()
    {
        var rendererComponents = mTrackableBehaviour.GetComponentsInChildren<Renderer>(true);
        var colliderComponents = mTrackableBehaviour.GetComponentsInChildren<Collider>(true);
        var canvasComponents = mTrackableBehaviour.GetComponentsInChildren<Canvas>(true);

        //ursprüngliches Objekt deaktivieren
        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;


        //Neues Objekt aktivieren
        // Enable rendering:
        foreach (var component in rendererComponents)
        {
            if (mood.Equals("building") && component.gameObject.tag.Equals("Building"))
            {
                Debug.Log("Building ");
                component.enabled = true;
            }
            else if (mood.Equals("piece") && component.gameObject.tag.Equals("Piece"))
            {
                Debug.Log("Piece");
                component.enabled = true;
            }
            else if (mood.Equals("info") && component.gameObject.tag.Equals("Info"))
            {
                Debug.Log("Info");
                component.enabled = true;
            }
        }

        // Enable colliders:
        foreach (var component in colliderComponents)
        {
            if (mood.Equals("building") && component.gameObject.tag.Equals("Building"))
            {
                Debug.Log("Building ");
                component.enabled = true;
            }
            else if (mood.Equals("piece") && component.gameObject.tag.Equals("Piece"))
            {
                Debug.Log("Piece");
                component.enabled = true;
            }
            else if (mood.Equals("info") && component.gameObject.tag.Equals("Info"))
            {
                Debug.Log("Info");
                component.enabled = true;
            }
        }

        // Enable canvas':
        foreach (var component in canvasComponents)
        {
            component.enabled = true;
        }
    }
}
