
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
    private Game game;
    private Player player;
    public KNearestNeighbor knn;
 

    public Game Game { get => game; set => game = value; }
    public Player Player { get => player; set => player = value; }

    public AIManager(AIType type)
    {
        aiType = type;
        actions[0] = Random.Range(50f, 60f);
        actions[1] = Random.Range(45f, 85f);
        bool[] annActions = new[] { false, false, false, false };
        if(type == AIType.KNN){
        knn = new KNearestNeighbor(3);
        knn.loadData();
        }
    }
    public enum AIType
    {
        SIMPLEREFLEX, REINFORCED, KNN
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
            actions[0] -= UnityEngine.Random.Range(-0.5f, 1f) * AIManager.sqrt(distToEnemy);//power
            actions[1] -= UnityEngine.Random.Range(-1f, 1f) * AIManager.sqrt(distToEnemy);//angle

            actions[0] = actions[0] > 100f ? actions[0] = 100 : actions[0];
            actions[0] = actions[0] < 0f ? actions[0] = 0 : actions[0];

            actions[1] = actions[1] > 89f ? actions[1] = 89 : actions[1];
            actions[1] = actions[1] < 1f ? actions[1] = 1 : actions[1];
        
                }
                break;
            case AIType.KNN:
                actions[0] = Random.Range(65f, 100f); ;//Power
                actions[1] = Random.Range(33f, 75f);//Angle

                 //actions = knn.getMove();//x y, wind
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