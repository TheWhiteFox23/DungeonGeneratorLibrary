using System;
using System.Collections.Generic;
using System.Text;

namespace DungeonGenerator
{
    class FloorSet
    {
        private List<int[]> FloorTilesSet = new List<int[]>();
        private int[] Center;
        private int Type = 0;
        private Dictionary<string, int> Borders = new Dictionary<string, int>();
        int ID;


        //CONSTRUCTORS
        public FloorSet()
        {

        }
        public FloorSet(List<int[]> FloorTilesSet)
        {
            this.FloorTilesSet = FloorTilesSet;
        }        
        public FloorSet(List<int[]> FloorTilesSet, int ID )
        {
            this.FloorTilesSet = FloorTilesSet;
            this.ID = ID;
        }
        public FloorSet(int Type)
        {
            this.Type = Type;
        }

        //GET - SET METHODS

            //FLOORSET
        public List<int[]> getFloorTilesSet()
        {
            return FloorTilesSet;
        }
        public void setFloorTilesSet(List<int[]> FloorTilesSet)
        {
            this.FloorTilesSet = FloorTilesSet;
        }
        public void addToFloorTilesSet(int[] ElementToAdd)
        {
            this.FloorTilesSet.Add(ElementToAdd);
        }        
        public void addToFloorTilesSet(List<int[]> ListToAdd)
        {
            foreach(var v in ListToAdd)
            {
                FloorTilesSet.Add(v);
            }
        }

            //CENTER
        public int[] getCenter()
        {
            return Center;
        }
        public void setCenter(int[] Center)
        {
            this.Center = Center;
        }

            //TYPE
        public int getType()
        {
            return Type;
        }
        public void setType(int Type)
        {
            this.Type = Type;
        }

            //BORDER
        public Dictionary<string, int> getBorders()
        {
            return Borders;
        }
        public void addBorder(string Key, int BorderFloorSet)
        {
            Borders.Add(Key, BorderFloorSet);
        }

        /*public List<int> getSurrondings()
        {
            return Borders.Values;
        }*/

            //ID
        public int getID()
        {
            return ID;
        }
    }
}
