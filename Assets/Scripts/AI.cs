
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    public AIType aiType;
    private float[] actions = new float[2] { 0.0f, 0.0f };
    private int score = 0;

    public AI(AIType type)
    {
        aiType = type;
        actions[0] = Random.Range(50f, 60f);
        actions[1] = Random.Range(45f, 85f);
    }
    public enum AIType
    {
        SIMPLEREFLEX , REINFORCED , ANN
    }

    public float[] getMove(float enemyDistance)
    {
        switch (aiType)
        {
            case AIType.SIMPLEREFLEX:
                actions[0] = Random.Range(0.0f, 100f);//Power
                actions[1] = Random.Range(0.0f, 180f);//Angle
                break;
            case AIType.REINFORCED:
                actions = train(enemyDistance);
               
                break;
            case AIType.ANN:
                break;
            default:
                actions[0] = Random.Range(0.0f, 100f);
                actions[1] = Random.Range(0.0f, 180f);
                break;
        }
        return actions;
    }
    private float[] train(float distToEnemy)
    {
        if (score <= 100)
        {

                if (distToEnemy != 0)
                {
                    score -= Mathf.Abs((int)distToEnemy);

                    actions[0]  += Random.Range(-0.5f, 1f) * distToEnemy*(-1);
                    actions[1]  += Random.Range(-0.5f, 1f) * distToEnemy*(-1);

                    actions[0] = actions[0] > 100f ? actions[0] = 100 : actions[0];
                    actions[0] = actions[0] < 0f ? actions[0] = 0 : actions[0];

                    actions[1] = actions[1] > 89f ? actions[1] = 89 : actions[1];
                    actions[1] = actions[1] < 1f ? actions[1] = 1 : actions[1];


                }
                else
                {
                    score += 20;
                }
            
        }

        return actions;
    }
}