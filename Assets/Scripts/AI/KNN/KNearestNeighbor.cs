// using System.Collections;
// using System.Collections.Generic;
// using System;
// using System.IO;
// using System.Runtime.Serialization;
// using System.Runtime.Serialization.Formatters.Binary;

// [Serializable]
// public class KNearestNeighbor
// {
//     private float[] actions = new float[] { 0.0f, 0.0f };
//     private String dataBase = "/Assets/Scripts/AI/ShotHistory.txt";
//     private String dataStor = "/Assets/Scripts/AI/KNN/Data/LastModel.dat";
//     private int k;
//     public Hashtable map;

//     public KNearestNeighbor(int k)
//     {
//         this.k = k; 
//         map = new Hashtable();
//         for (int i = -10; i < 11; i++)
//         {
//             map.Add(i, new List<Data>());
//         }
//     }
//     public Hashtable getMap()
//     {
//         return map;
//     }
    
//     public void loadData()
//     {
//         using (StreamReader reader = new StreamReader(dataBase))
//         {
//             string line;
//             string[] tokens;
//             bool firstline = true;
//             while ((line = reader.ReadLine()) != null)
//             {
//                 if (firstline)
//                 {
//                     firstline = false;
//                 }
//                 else
//                 {
//                     tokens = line.Split('|');
//                     int wind = Int32.Parse(tokens[2]);
//                     map[wind].Add(new Data(float.Parse(tokens[0]), float.Parse(tokens[1]), wind, float.Parse(tokens[3]), float.Parse(tokens[4])));
//                 }
//             }
//         }
//     }

//     public void loadModel()
//     {
//         IFormatterConverter formatter = new BinaryFormatter();
//         Stream stream = new FileStream(dataStor, FileMode.Open, FileAccess.Read);
//         this.map = (Hashtable)formatter.Deserialize(stream).getMap();
//         stream.Close();
//     }
//     public void saveModel()
//     {
//         IFormatterConverter formatter = new BinaryFormatter();
//         Stream stream = new FileStream(dataStor, FileMode.Create, FileAccess.Write);
//         formatter.Serialize(stream, this);
//         stream.Close();
//     }


//     public class Data : IComparable<Data>
//     {
//         public int category { get => category; set => category; }
//         public int wind { get => wind; set => wind; }
//         public float power { get => power; set => power; }
//         public float angle { get => angle; set => angle; }
//         public float xDist { get => xDist; set => xDist; }
//         public float yDist { get => yDist; set => yDist; }
//         public Data(float xDiff, float yDiff, int wind, float angle, float power)
//         {
//             this.xDist = xDiff;
//             this.yDiff = yDiff;
//             this.wind = wind;
//             this.angle = angle;
//             this.power = power;
//             this.category = (xDiff * yDiff) / 100;
//         }
//         public int CompareTo(Data other)
//         {
//             return other == null ? 1 : category > other.category ? 1 : category < other.category ? -1 : 0;
//         }
//     }
// }
