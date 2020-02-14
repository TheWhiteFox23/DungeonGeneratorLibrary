using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator

{
    class Room2
    {
        //id number - smallest is 1, 0 is reserved for walls;
        Dictionary<string, int> border2 = new Dictionary<string, int>();
        int id;
        //Map of neighborts and border with them
        Dictionary<int, List<string>> neighbours = new Dictionary<int, List<string>>();

        List<int> conections = new List<int>();

        public Room2(int id)
        {
            this.id = id;
            conections.Add(id);
        }

        //Overload - test of git
        public Room2()
        {
            id = 0;
            conections.Add(id);
        }


        // GET and SET methosd

        public int GetID()
        {
            return id;
        }

        public List<int> GetSurrounding()
        {
            return neighbours.Keys.ToList();
        }

        public Dictionary<int, List<string>> GetBorderMap()
        {
            return neighbours;
        }

        public void MergeWith2(Room2 roomToMerge, string border)
        {

            border2.Remove(border);

            conections.Add(roomToMerge.GetID());


            foreach(var n in roomToMerge.GetBorderMap())
            {
                if (!neighbours.ContainsKey(n.Key))
                {
                    neighbours.Add(n.Key, n.Value);
                }
                else
                {
                    var l = neighbours[n.Key];
                    foreach(var i in n.Value)
                    {
                        if(i != border)
                        {
                            l.Add(i);
                        }
                        
                    }
                    neighbours[n.Key] = l;
                }
                
            }
            foreach(var c in conections)
            {
                if(neighbours.ContainsKey(c))neighbours.Remove(c);
            }
        }

        public void AddBorder2(string border, int neighbourt)
        {
            if(!border2.ContainsKey(border))border2.Add(border, neighbourt);
            if (neighbours.ContainsKey(neighbourt))
            {
                List<string> add = neighbours[neighbourt];
                add.Add(border);
                neighbours[neighbourt] = add;
            }
            else
            {
                List<string> add = new List<string>();
                add.Add(border);
                neighbours.Add(neighbourt, add);
            }

        }

        public Dictionary<string, int> GetBorder2()
        {
            return border2;

        }
    }
}