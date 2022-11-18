
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager
{
    public AIType aiType;
    public float distanceBetweenPlayers;
    [Tooltip("actions[0] = Power | actions[1] = Angle")]
    private float[] actions = new float[2] { 0.0f, 0.0f };
    private int score = 0;
    private NeuralNetFF x;

    public AIManager(AIType type)
    {
        aiType = type;
        actions[0] = Random.Range(50f, 60f);
        actions[1] = Random.Range(45f, 85f);
        bool[] annActions = new[] { false, false, false, false };
        if (type == AIType.ANN)
        {
            x = new NeuralNetFF(2, 2, 3, 4, ActivationFunctions.Functions.SIGMOID);
            x.feedForward();
        }
    }
    public enum AIType
    {
        SIMPLEREFLEX, REINFORCED, ANN
    }


    public float[] getMove(float distToEnemy)
    {
        switch (aiType)
        {
            case AIType.SIMPLEREFLEX:
                actions[0] = Random.Range(65f, 100f); ;//Power
                actions[1] = Random.Range(33f, 75f);//Angle

                /*  Original: commented out at 7:53 11/13/2022
                 * actions[0] = Random.Range(0.0f, 100f);//Power
                 * actions[1] = Random.Range(0.0f, 180f);//Angle*/
                break;
            case AIType.REINFORCED:
                if (distToEnemy != 0)
                {
                    score -= Mathf.Abs((int)distToEnemy);

                    actions[0] -= Random.Range(-0.75f, 1.5f) * sqrt(distToEnemy);
                    actions[1] -= Random.Range(-.75f, .75f) * sqrt(distToEnemy);

                    actions[0] = actions[0] > 100f ? actions[0] = 100 : actions[0];
                    actions[0] = actions[0] < 0f ? actions[0] = 0 : actions[0];

                    actions[1] = actions[1] > 75 ? actions[1] = 75 : actions[1];
                    actions[1] = actions[1] < 1f ? actions[1] = 1 : actions[1];
                }
                break;
            case AIType.ANN:
                //just testing 0_o
                x.setRealInput(actions); x.feedForward();
                actions[0] = x.getRealOutput()[0] * 90;
                actions[1] = x.getRealOutput()[1] * 90;
                x.setFitness(distToEnemy != 0 ? x.getFitness() - 10 : x.getFitness() + 10);
                if (distToEnemy != 0)
                    x.mutate();
                break;
            default:
                actions[0] = Random.Range(0.0f, 100f);
                actions[1] = Random.Range(45f, 50f);
                break;
        }
        return actions;
    }

    public static float sqrt(float val)
    {
        float output = Mathf.Sqrt(Mathf.Abs(val));

        if (val < 0f)
            return -output;

        return output;

    }
}