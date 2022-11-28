using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class KNearestNeighbor
{
    private string dataBase = "Assets/Scripts/AI/ShotHistory.txt";
    private string dataStor = "Assets/Scripts/AI/KNN/Data/LastModel.dat";
    private int k;
    public Hashtable map;

    public KNearestNeighbor(int k)
    {
        this.k = k < 3 ? 3 : k > 10 ? 10 : k;
        map = new Hashtable();
        for (int i = -10; i < 11; i++)
        {

            map.Add(i, new Hashtable());
        }
    }
    public float[] getMove(float xDist, float yDist, int wind)
    {
        int category = (int)(xDist * yDist) / 100;
        List<Data> shotHist = (List<Data>)map[wind];
        shotHist.Sort();
        float[] actions = new float[] { 0.0f, 0.0f };
        for (int i = 0; i < shotHist.Count; i++)
        {
            if (shotHist[i].category == category)
            {
                actions = new float[] { shotHist[i].getPower(), shotHist[i].getAngle() };
                return actions;
            }
            else if (i < shotHist.Count && i > 0)
            {
                Data prev = shotHist[i - 1];
                Data next = shotHist[i + 1];
                if (prev.category < category && next.category > category)
                {

                    for (int j = 0; j < k; j++)
                    {
                        try
                        {
                            actions[0] +=  shotHist[i + j].getPower();
                             actions[1] += shotHist[i + j].getAngle();
                        }
                        catch (Exception e)
                        {
                        }

                        try
                        {
                            actions[0] +=  shotHist[i - j].getPower();
                            actions[1] +=  shotHist[i - j].getAngle();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    actions[0] = actions[0] / k;
                    actions[1] = actions[1] / k;
                    return actions;
                }
            }
            else if (i < shotHist.Count)
            {
                for (int j = 0; j < k; j++)
                {
                    try
                    {
                        actions[0] += (shotHist[i +/*-*/ j].getPower());
                        actions[1] += (shotHist[i +/*-*/ j].getAngle());
                    }
                    catch(Exception e )
                    {

                    }
                }
                actions[0] = actions[0] / k;
                actions[1] = actions[1] / k;
                return actions;
            }
        }
        return actions;
    }
    public Hashtable getMap()
    {
        return map;
    }

    public void loadData()
    {
        using (StreamReader reader = new StreamReader(dataBase))
        {
            string line;
            string[] tokens;
            bool firstline = true;
            while ((line = reader.ReadLine()) != null)
            {
                if (firstline)
                {
                    firstline = false;
                }
                else
                {
                    tokens = line.Split('|');
                    if (tokens != null && tokens.Length > 2)
                    {
                        int wind = Int32.Parse(tokens[2]);
                        float x =float.Parse(tokens[0]);
                        float y = float.Parse(tokens[1]);
                        Hashtable temp = (Hashtable)map[wind];
                        if(!map[wind].Equals((x*y)/Math.Abs(x))) //Needs to be looked at
                        {
                            temp.Add((x*y)/Math.Abs(x),new Data(float.Parse(tokens[0]), float.Parse(tokens[1]), wind, float.Parse(tokens[3]), float.Parse(tokens[4])));
                        }
                        map[wind] = temp;
                    }
                }
            }
        }
    }

    public void loadModel()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(dataStor, FileMode.Open, FileAccess.Read);
        KNearestNeighbor temp = (KNearestNeighbor)formatter.Deserialize(stream);
        this.map = temp.getMap();
        stream.Close();
    }
    public void saveModel()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(dataStor, FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, this);
        stream.Close();
    }


    public class Data : IComparable<Data>
    {
        public int category, wind,count;
        public float power, angle, xDist, yDist;
        public Data(float xDist, float yDist, int wind, float angle, float power)
        {
            this.xDist = xDist;
            this.yDist = yDist;
            this.wind = wind;
            this.angle = angle;
            this.power = power;
            this.category = (int)(xDist * yDist) / 100;
            this.count = 0;
        }
        public void addData(float power, float angle){
            count++;
            this.power += power;
            this.angle += angle;

        }
        public void averageData(){
            this.power /= count;
            this.angle /= count;
        }
        public float getPower()
        {
            return power;
        }
        public float getAngle()
        {
            return angle;
        }

        public int CompareTo(Data other)
        {
            return other == null ? 1 : category > other.category ? 1 : category < other.category ? -1 : 0;
        }
    }
}
