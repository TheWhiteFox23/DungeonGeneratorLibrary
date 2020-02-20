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

        public FloorSet()
        {

        }

        public FloorSet(List<int[]> FloorTilesSet)
        {
            this.FloorTilesSet = FloorTilesSet;
        }        
        public FloorSet(List<int[]> FloorTilesSet, int Type)
        {
            this.FloorTilesSet = FloorTilesSet;
            this.Type = Type;
        }

        public FloorSet(int Type)
        {
            this.Type = Type;
        }
    }
}
