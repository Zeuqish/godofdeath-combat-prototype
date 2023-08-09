using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class EnemyAISO : ScriptableObject
{
    [SerializeField] ActionSO[] enemyActions;
    private int[] actionWeights = new int[6];
    private int totalWeight = 0;
    private int hostility = 0;
    public ActionSO getAction(int hostility, int orbsBroken)
    {
        
        if (hostility != this.hostility)
        {
            this.hostility = hostility;
            actionWeights[0] = 80 - this.hostility;
            actionWeights[1] = 100 - this.hostility;
            actionWeights[2] = this.hostility/2;
            actionWeights[3] = this.hostility/2;
            actionWeights[4] = (this.hostility - 60) + (orbsBroken * 5);
            actionWeights[5] = (this.hostility - 80) + (orbsBroken * 5);
            totalWeight = actionWeights.Sum();
        }
        int choice = Random.Range(1, totalWeight);
        for (int i = 0; i< actionWeights.Length; i++) {
            choice -= actionWeights[i];
            if (choice <= 0)
            {
                return enemyActions[i];
            }
        }
        return enemyActions[0];
        
    }
}
