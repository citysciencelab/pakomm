using System;
using System.Numerics;
using Normal.Realtime;
using TMPro;
using TouchScript.Behaviors;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.Interaction.Toolkit;
using Vector3 = UnityEngine.Vector3;

public class DatabaseSyncAnnotation : RealtimeComponent<DatabaseAnnotationModel>
{
    public string id => model.id;
    public string visualId;
    public string visualContent;
    public int visualUpVotes;
    public int visualDownVotes;
    public DateTime visualCreatedTime;
    public string _objectid => model.parentObjectId;
    public string visualParentId; 
    public string content => model.content;
    public int upvotes => model.voteUp;
    public int downvotes => model.voteDown;


    private void Start()
    {
        // Steuerung für Multitouch und Ipad
        if (!Manager.GameManager.VRMode)
        {
            AnnotationList.AL.ConstructAndShowScrollView();
        }

        if (Manager.GameManager.VRMode)
        {
           
        }
        
        

    }
    
    
    protected override void OnRealtimeModelReplaced(DatabaseAnnotationModel previousModel, DatabaseAnnotationModel currentModel)
    {

        if (realtimeView.isSceneView)
        {
            Destroy(this);
            return;
        }

        if (previousModel != null)
        {
            // Unregister from events
            previousModel.idDidChange -= IdDidChange;
            previousModel.contentDidChange -= ContentDidChange; 
            previousModel.voteUpDidChange -= VoteUpDidChange;
            previousModel.voteDownDidChange -= VoteDownDidChange;
            previousModel.parentObjectIdDidChange -= ParentIdChange; 
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                if (GetComponent<RealtimeView>().isOwnedLocallyInHierarchy)
                {
                    model.id = "";
                    visualId = model.id;
                }
            }

            UpdateID();
            UpdateContent();
            UpdateVotesUp();
            UpdateVotesDown();
            UpdateParentId();

            currentModel.idDidChange += IdDidChange;
            currentModel.contentDidChange += ContentDidChange;
            currentModel.voteUpDidChange += VoteUpDidChange; 
            currentModel.voteDownDidChange += VoteDownDidChange;
            currentModel.parentObjectIdDidChange += ParentIdChange; 
        }
        
    }

    private void ParentIdChange(DatabaseAnnotationModel databaseAnnotationModel, string value)
    {
        UpdateParentId();
    }

    private void UpdateParentId()
    {
        visualParentId = model.parentObjectId;
        gameObject.name = model.parentObjectId;
        
        
    }

    private void IdDidChange(DatabaseAnnotationModel model, string id)
    {
        UpdateID();
        Debug.Log(" Update EVENT DID ID CHANGE  " + id);
    }
    
    
    private void ContentDidChange(DatabaseAnnotationModel model, string content)
    {
        UpdateContent();
        Debug.Log(" Update EVENT DID ID CHANGE  " + content);
    }
    
    private void VoteUpDidChange(DatabaseAnnotationModel model, int voteUp)
    {
        UpdateVotesUp();
        Debug.Log(" Vote Up Changed " + voteUp);
       
    }
    private void VoteDownDidChange(DatabaseAnnotationModel model, int voteDown)
    {
        UpdateVotesDown();
        Debug.Log(" Vote DOWN Changed " + voteDown);
       
    }
    
    private void UpdateID()
    {
        // Update the UI
        visualId = model.id;
        Debug.Log( id + "ID CHANGED");
    }

    private void UpdateContent()
    {
        visualContent = model.content;
        Debug.Log(content + " Content ");
    }
    
    
    private void UpdateVotesUp()
    {
        visualUpVotes = model.voteUp;
        
        if (!Manager.GameManager.VRMode)
        {
            if (AnnotationList.AL != null)
            {
                Debug.Log("Refresh AL Reached");
                AnnotationList.AL.ConstructAndShowScrollView();
            }
            
        }
        
        Debug.Log("VOTES UP CHANGED");
    }
    
    private void UpdateVotesDown()
    {
        visualDownVotes = model.voteDown;
       
        
        if (!Manager.GameManager.VRMode)
        {
            if (AnnotationList.AL != null)
            {
                Debug.Log("Refresh AL Reached");
                AnnotationList.AL.ConstructAndShowScrollView();
            }
            
        }
        
        Debug.Log("VOTES DOWN CHANGED");

      

    }
    public void DeleteAnnotationObject()
    {
        Debug.Log("Delete Object reached");
        if (id != "" || id != null || id != " ")
        {
            DgraphQuery.DQ.deleteObjectById(this.gameObject, model.id);
            Debug.Log("Requested DELETE WITH ID " + model.id);
        }
        
        else
        {
            Debug.Log("NO ID CANT DELETE WITHOUT BREAKING DATABASE SYNC");
        }
    }

    public void SetId(string id)
    {
        model.id = id;
    }
    
    public void SetContent(string content)
    {
        model.content = content; 
    }

    public int upVote()
    {
        this.GetComponent<RealtimeView>().RequestOwnership();
        Debug.Log("REACHING UPVOTE"); 
        visualUpVotes += 1;
        DgraphQuery.DQ.updateAnnotation(id, visualDownVotes , visualUpVotes);
        model.voteUp = visualUpVotes;
        return visualUpVotes;
       
    }
    
    public int downVote()
    {
        this.GetComponent<RealtimeView>().RequestOwnership();
        Debug.Log("REACHING DOWNVOTE"); 
        visualDownVotes +=  1;
        DgraphQuery.DQ.updateAnnotation(id, visualDownVotes, visualUpVotes);
        model.voteDown = visualDownVotes; 
        return visualDownVotes;
        

    }


    public void SetAnnotation(string id, string content, int up, int down, DateTime _dateTime, string objId)
    {
        model.id = id;
        visualId = id; 
        model.content = content;
        visualContent = content; 
        model.voteUp = up;
        visualUpVotes = up; 
        model.voteDown = down;
        visualDownVotes = down; 
        visualCreatedTime = _dateTime;
        model.parentObjectId = objId;
        visualParentId = objId;
        
        if (!Manager.GameManager.VRMode)
        {
            if (AnnotationList.AL != null)
            {
                AnnotationList.AL.ConstructAndShowScrollView();
            }
            
        }
       
        //Debug.Log("SYNING " + model.parentObjectId );
        //Debug.Log(_dateTime);
    }
}
