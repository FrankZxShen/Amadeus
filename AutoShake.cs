using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoShake : MonoBehaviour
{
    public int[] eyesID;
    public float mintime;
    public float maxtime;
    private float timetarget;
    public float timeTick;
    public float timeshake;
    
    public SkinnedMeshRenderer[] eyes;
    void start()
    {
        ResetTimeTarget();
    }

    private void ResetTimeTarget()
    {
        timetarget=Random.Range(mintime,maxtime);//随机眨眼的时间
    }
    
    void Update()
    {
        timeTick+=1*Time.deltaTime;//超过等待时间开始眨眼
        if(timeTick>timetarget)
        {
            for(int c=0;c<eyes.Length;c++)  
            {
                for(int i=0;i<eyesID.Length;i++)
                {
                    eyes[c].SetBlendShapeWeight(eyesID[i],Mathf.Lerp(eyes[c].GetBlendShapeWeight(eyesID[i]),100,0.3f));
                }
            }
        }
        if(timeTick>timetarget+timeshake)
        {
            timeTick=0;
            ResetTimeTarget();
        }
        else
        {
            for(int c=0;c<eyes.Length;c++)
            {
                for(int i=0;i<eyesID.Length;i++)
                {
                    eyes[c].SetBlendShapeWeight(eyesID[i],Mathf.Lerp(eyes[c].GetBlendShapeWeight(eyesID[i]),0,0.1f));
                }
            }
        }
    }
}
