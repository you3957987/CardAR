using UnityEngine;
using Vuforia;

public class TrackingObject : MonoBehaviour
{
    public TextMesh obj_text_mesh_;
    public string name_;
    public int hp_;
    
    // 모델 교체용 배열(프리팹 또는 씬의 GameObject)
    public GameObject[] models;
    // 0은 기본 모델
    public int currentModelIndex = 0;
    
    private ObserverBehaviour mTrackableBehaviour;
    public bool is_detected = false;
    
    void Start()
    {
        obj_text_mesh_.text = name_ + "\n HP:" + hp_;
        mTrackableBehaviour = GetComponent<ObserverBehaviour>();
        if (mTrackableBehaviour) 
        {
            mTrackableBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
            OnTargetStatusChanged(mTrackableBehaviour, mTrackableBehaviour.TargetStatus);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        if (targetStatus.Status == Status.TRACKED)
        {
            is_detected = true;
        }
        else
        {
            is_detected = false;
        }
    }
    
}
