using UnityEngine;
using BehaviorTree;
using Unity.VisualScripting;

public class FireMist : Node
{
    GameObject mist;
    float mistFireTime = 3f;
    float mistTime = 3f;
    public FireMist(GameObject mist)
    {
        this.mist = mist;
    }

    public override NodeState Evaluate()
    {
        if (!mist.activeInHierarchy)
        {
            mist.SetActive(true);
            mist.GetComponent<ParticleSystem>().Play();
        }
        state = NodeState.SUCCESS;
        return state;
    }
}
