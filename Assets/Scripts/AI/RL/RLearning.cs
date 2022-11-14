using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class RLearning :IComparable<RLearning>
{
    private float[] actions = new float[2] { 0.0f, 0.0f };
    private  String dataStor = "Assets/Scripts/AI/R/Data/rl.data";
    public int score = 0;
    public int wind= 0;

    public RLearning()
    {
    }

    public float[] getMove(float distToEnemy ,int wind )
    {
        this.wind = wind;
        if (distToEnemy != 0)
         {
            score -= Mathf.Abs((int)distToEnemy);
    
            actions[0] -= UnityEngine.Random.Range(-0.5f, 1f) * AIManager.sqrt(distToEnemy);//power
            actions[1] -= UnityEngine.Random.Range(-1f, 1f) * AIManager.sqrt(distToEnemy);//angle

            actions[0] = actions[0] > 100f ? actions[0] = 100 : actions[0];
            actions[0] = actions[0] < 0f ? actions[0] = 0 : actions[0];

            actions[1] = actions[1] > 89f ? actions[1] = 89 : actions[1];
            actions[1] = actions[1] < 1f ? actions[1] = 1 : actions[1];
        }
        return actions;
    }
    
     public int CompareTo(RLearning other)
    {
        if (other == null) return 1;
        if (score > other.score)
            return 1;
        else if (score < other.score)
            return -1;
        else
            return 0;
    }

    // private Boolean[] LoadAction(string path)
    // {
    //     TextReader tr = new StreamReader(path);
    //     int NumberOfLines = (int)new FileInfo(path).Length;
    //     string[] ListLines = new string[NumberOfLines];
    //     for (int i = 1; i < NumberOfLines; i++)
    //     {
    //         ListLines[i] = tr.ReadLine();
    //     }
    //     tr.Close();
    //     String []line;
    //     String bestStats;
    //     foreach(String x in ListLines){
    //         line = x.Split(' ');
    //         int prevWind = Int32.Parse(line[1]);
    //         if(prevWind == wind)
    //         {
    //             // float score  == line[2];
    //             //  if(score > this.score )
    //             //  {

    //             //  }
    //         }
    //     }


    // //  return null;
   // //}

    /*public boolean Load(string path)
    {
        
    }*/

    public void Save(string path)
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);
        String playData = "wind: " + wind + " " + score + " " + actions[0] + " " + actions[1];
        writer.Write(playData);
        writer.Close();
    }
    
}
