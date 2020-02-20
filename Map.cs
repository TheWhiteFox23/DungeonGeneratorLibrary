using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGenerator
{
    public class Map
    {
        int[][] Grid;
        readonly int Height;
        readonly int Width;
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        bool diagnostics = true;
        bool generateImage = true;
        int seed = 4098;
        Random random;
        Dictionary<int, FloorSet> IndividualRooms = new Dictionary<int, FloorSet>();

        int Reserved = 20;

        //RESERVED SYMBOLS - 0 to 20
        readonly int BORDER = 0;
        readonly int UNMARKFLOOR = 1;
        readonly int WALL = 2;
        readonly int DOOR = 3;
        readonly int CENTER = 4;

        



        public Map(int Width, int Height)
        {
            this.Height = Height;
            this.Width = Width;
            Grid = new int[Height][];
            random = new Random(seed);
            OnCreate();
        }

        private void OnCreate()
        {
            InitializeGrid();
            //GenerateRoomsRandom();
            //SeedingTest();
            GenerateRoomTightSquares(3, 15);
            MapRooms();
            MapBorders(IndividualRooms,Grid,WALL);
            TestBorders();
            //TestRooms();
            //DeleteSingles();
            //FloodFillCorners(int.MaxValue, 0, 0, 0, Grid);
            //DeleteInnerBlockWalls();
            //MapRoomsFixed(1);
            BufferImage();
        }

        private void InitializeGrid()
        {
            StopWatchStart();
            for (int i = 0; i< Height; i++)
            {
                int[] add = new int[Width];
                //Array.Copy(help, 0, add, 0, Width);
                Grid[i] = add;
            }
            StopWatchStop("InitializeGrid");
        }


        //PUBLIC

            //GRID FILLING ALGORITHMS
        public void GenerateRoomsRandom()
        {
            StopWatchStart();
            //Random random = new Random(seed);
            for(int i = 1; i<Height-1; i++)
            {
                for(int j = 1; j< Width-1; j++)
                {
                    Grid[i][j] = random.Next(0, 2);
                }
            }
            StopWatchStop("GenerateRoomsRandom");

        }
        public void GenerateRoomTightSquares(int MinRoomSize, int MaxRoomSize)
        {
            StopWatchStart();
            Dictionary<string, int[]> ValidStarts = new Dictionary<string, int[]>();
            //find all valid starts
            for(int i = 1; i<Height-1; i++)
            {
                for(int j = 1; j< Width-1; j++)
                {
                    ValidStarts.Add(i + "." + j, new int[] { i, j});
                }
            }

            //Filling with rooms
            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width- 1; j++)
                {
                    if (ValidStarts.ContainsKey(i +"."+ j))
                    {
                        int H = random.Next(MinRoomSize, MaxRoomSize);
                        int W = random.Next(MinRoomSize, MaxRoomSize);
                        for(int a = 0; a < H; a++)
                        {
                            for(int b = 0; b< W; b++)
                            {
                                int indexX = j + b;
                                int indexY = i + a;
                                if (indexX > 0 && indexY > 0 && indexX < Width - 1 && indexY < Height - 1 && ValidStarts.ContainsKey(indexY + "." + indexX))
                                {
                                    ValidStarts.Remove(indexY + "."  + indexX);
                                    Grid[indexY][indexX] = UNMARKFLOOR;
                                }
                            }
                        }
                        
                        for(int a = j-1; a<= j + W; a++)
                        {
                            int iX = a;
                            int iY = (i - 1);
                            string border = iY + "." + iX;
                            if (iX > 0 && iY > 0 && iX < Width - 1 && iY < Height - 1 && ValidStarts.ContainsKey(border))
                            {
                                ValidStarts.Remove(border);
                                Grid[iY][iX] = WALL;
                            }

                        }
                        for (int a = j - 1; a <=j + W; a++)
                        {
                            int iX = a;
                            int iY = (i + H);
                            string border = iY + "." + iX;
                            if (iX > 0 && iY > 0 && iX < Width - 1 && iY < Height - 1 && ValidStarts.ContainsKey(border))
                            {
                                ValidStarts.Remove(border);
                                Grid[iY][iX] = WALL;
                            }
                        }
                        for (int a = i - 1; a <= i +H ; a++)
                        {
                            int iX = j-1;
                            int iY = (a);
                            string border = iY + "." + iX;
                            if (iX > 0 && iY > 0 && iX < Width - 1 && iY < Height - 1 && ValidStarts.ContainsKey(border))
                            {
                                ValidStarts.Remove(border);
                                Grid[iY][iX] = WALL;
                            }
                        }
                        for (int a = i - 1; a <=i+ H; a++)
                        {
                            int iX = j+W;
                            int iY = a;
                            string border = iY + "." + iX;
                            if (iX > 0 && iY > 0 && iX < Width - 1 && iY < Height - 1 && ValidStarts.ContainsKey(border))
                            {
                                ValidStarts.Remove(border);

                                Grid[iY][iX] = WALL;
                            }
                        }
                    }
                }
            }
            StopWatchStop("GenerateRoomTightSquares");
        }

            //MAPING METHODS
        public void MapRooms()
        {
            StopWatchStart();
            int ID = 21;
            //storing all tiles and indexes of the room
            for (int i = 1; i < Height-1; i++)
            {
                for (int j = 1; j < Width-1; j++)
                {
                    if (Grid[i][j] == 1)
                    {
                        FloorSet newFloorSet = new FloorSet(FloodFill(ID, j, i, 1, Grid, false), ID);
                        //FloodFill(ID, j, i, 1, Grid, false);
                        IndividualRooms.Add(ID, newFloorSet);
                        ID++;
                    }
                    
                }
            }
            StopWatchStop("MapRooms");
        }
        public void MapRooms(int MinSize)
        {
            StopWatchStart();
            int ID = 21;
            //storing all tiles and indexes of the room
            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    if (Grid[i][j] == 1)
                    {
                        List<int[]> FloorTiles = FloodFill(ID, j, i, 1, Grid, false);
                        FloorSet newFloorSet = new FloorSet(FloodFill(ID, j, i, 1, Grid, false), ID);
                        //FloodFill(ID, j, i, 1, Grid, false);
                        IndividualRooms.Add(ID, newFloorSet);

                        if (FloorTiles.Count() < MinSize)
                        {
                            //Console.WriteLine(FloorTiles.Count());
                            foreach(var f in FloorTiles)
                            {
                                Grid[f[1]][f[0]] = 0;
                                IndividualRooms.Remove(ID);
                            }
                            //PrintAsCharArray();
                            //Console.ReadKey();
                        }
                    }
                    ID++;
                }
            }
            StopWatchStop("MapRooms");

        }
        public void MapRoomsFixedID(int FixedID)
        {
            StopWatchStart();
            //storing all tiles and indexes of the room
            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    if (Grid[i][j] != 0 && Grid[i][j] != FixedID)
                    {
                        List<int[]> FloorTiles = FloodFill(FixedID, j, i, Grid[i][j], Grid, false);
                    }
                }
            }
            StopWatchStop("MapRoomsFixed");

        }

            //DEBUGING AND PRINTING
        public void BufferImage()
        {
            StopWatchStart();
            if (generateImage)
            {
                Dictionary<int, byte[]> ColorPallete = new Dictionary<int, byte[]>();
                ColorPallete.Add(0, new byte[] { 0, 0, 0 });
                ColorPallete.Add(2, new byte[] { 0, 0, 0 });
                ImageBuffer image = new ImageBuffer(Width, Height);
                for (int i = 0; i < Height; i++)
                {
                    //Random random = new Random(seed);
                    for (int j = 0; j < Width; j++)
                    {
                        if (ColorPallete.ContainsKey(Grid[i][j]))
                        {
                            byte[] RGB = ColorPallete[Grid[i][j]];
                            image.PlotPixel(j, i, RGB[0], RGB[1], RGB[2]);
                        }
                        else
                        {
                            byte[] RGB = new byte[3];
                            RGB[0] = (byte)random.Next(0, 255);
                            RGB[1] = (byte)random.Next(0, 255);
                            RGB[2] = (byte)random.Next(0, 255);
                            image.PlotPixel(j, i, RGB[0], RGB[1], RGB[2]);
                            ColorPallete.Add(Grid[i][j], RGB);
                        }
                    }
                }
                image.save();
            }
            StopWatchStop("BufferImage");
        }
        public void PrintAsCharArray()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (Grid[i][j] == 0 || Grid[i][j] == 2)
                    {
                        Console.Write("{0} ", 'o');
                    }
                    else
                    {
                        Console.Write("{0} ", ' ');
                    }
                }
                Console.WriteLine();
            }
        }

            //CLEANUP
        public void DeleteInnerBlockWalls()
        {
            StopWatchStart();
            //Left to Right Direction
            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    if (Grid[i][j] == 0)
                    {
                        int[] W = { i, j - 1 };
                        int[] E = { i, j };
                        while (E[1] < Width && Grid[E[0]][E[1]] == 0)
                        {
                            E[1]++;
                            j++;
                        }
                        if (E[1] < Width && Grid[W[0]][W[1]] == Grid[E[0]][E[1]])
                        {
                            for (int d = W[1] + 1; d < E[1]; d++)
                            {
                                if (i + 1 < Height && i - 1 >= 0 && (Grid[i - 1][d] == 0 || Grid[i - 1][d] == Grid[W[0]][W[1]]) && (Grid[i + 1][d] == 0 || Grid[i + 1][d] == Grid[W[0]][W[1]]))
                                {
                                    Grid[i][d] = Grid[W[0]][W[1]];
                                    IndividualRooms[Grid[W[0]][W[1]]].addToFloorTilesSet(new int[] { d, i });
                                }

                            }
                        }
                    }

                }
            }
            //Top Down dirrection
            for (int j = 1; j < Width - 1; j++)
            {
                for (int i = 1; i < Height - 1; i++)
                {
                    if (Grid[i][j] == 0)
                    {
                        int[] W = { i - 1, j };
                        int[] E = { i, j };
                        while (E[0] < Height && Grid[E[0]][E[1]] == 0)
                        {
                            E[0]++;
                            i++;
                        }
                        if (E[0] < Height && Grid[W[0]][W[1]] == Grid[E[0]][E[1]])
                        {
                            for (int d = W[0] + 1; d < E[0]; d++)
                            {
                                if (i + 1 < Width && i - 1 >= 0 && (Grid[d][j - 1] == 0 || Grid[d][j - 1] == Grid[W[0]][W[1]]) && (Grid[d][j + 1] == 0 || Grid[d][j + 1] == Grid[W[0]][W[1]]))
                                {
                                    Grid[d][j] = Grid[W[0]][W[1]];
                                    IndividualRooms[Grid[W[0]][W[1]]].addToFloorTilesSet(new int[] { d, i });
                                }

                            }
                        }
                    }

                }
            }

            StopWatchStop("DeleteInnerBlockWalls");
        }

        //ROOM CONECTION
        //private void ConectRooms2()
        //{
        //    /**Room Connect algorith destciption
        //     * 1, Select room - separate it from array
        //     * 2, Find all of the neghbourt of the room and randomly choose one
        //     * 3, selectst random border of the room - merge with current room and delete from list
        //     * 4, add current room to the list
        //     */
        //    //List<FloorSet> rooms = IndividualRooms.Values.ToList();
        //    //randomly choosing room
        //    Random random = new Random();
        //    int count = IndividualRooms.Count();

        //    int rand = random.Next(2, count + 1);
        //    var megaRoom = IndividualRooms[rand];


        //    while (IndividualRooms.Count() != 1)
        //    {
        //        IndividualRooms.Remove(megaRoom.getID());
        //        //Choose one random neighbour and conect
        //        Room2 neighboutr = new Room2(); //empty constructor (ID is zero and all tiles are empty);
        //        int roomToChange = 0;

        //        if (megaRoom.getSurrounding().Count != 0)
        //        {
        //            roomToChange = megaRoom.GetSurrounding().First();
        //        }
        //        else
        //        {
        //            Console.WriteLine("Error ocured during room maping");
        //            break;
        //        }
        //        string border = megaRoom.GetBorderMap()[roomToChange].First();



        //        int coordinateX = ParseMap(border)[0];
        //        int coordinateY = ParseMap(border)[1];
        //        dMap[coordinateY][coordinateX] = door;
        //        roomMap[coordinateY][coordinateX] = int.MaxValue;
        //        roomTypeMap[coordinateY][coordinateX] = int.MaxValue;

        //        //merging
        //        megaRoom.MergeWith2(rooms2[roomToChange], border);
        //        rooms2.Remove(roomToChange);
        //        rooms2.Add(megaRoom.GetID(), megaRoom);
        //    }
        //}


        //DEBUGING METHODS
        private void StopWatchStart()
        {
            if (diagnostics)
            {
                watch.Start();
            }
        }
        private void StopWatchRestart(string testedProcedure)
        {
            if (diagnostics)
            {
                Console.WriteLine("Procedure --- {0} --- finished execution in time : {1}", testedProcedure, watch.Elapsed);
                watch.Restart();
            }
        }
        private void StopWatchStop(string testedProcedure)
        {
            if (diagnostics)
            {
                Console.WriteLine("Procedure --- {0} --- finished execution in time : {1}", testedProcedure, watch.Elapsed);
                watch.Reset();
                watch.Stop();
            }
        }
        private void TestRooms()
        {
            string roomText = "";
            foreach (FloorSet f in IndividualRooms.Values)
            {
                roomText += ("Room with ID:" + f.getID() + "has following tiles ---");

                //Console.Write("Room with ID: {0}  has following tiles ---", f.getID());
                //System.IO.File.WriteAllText(@"RoomsAndTiles.txt", roomText);
                foreach (var t in f.getFloorTilesSet())
                {
                    roomText += ("X:" + t[0] + " --- Y:" + t[1] + "    ");
                    //Console.Write("X:" +t[0]+ " --- Y:"+t[1]+"    ");
                    //System.IO.File.WriteAllText(@"RoomsAndTiles.txt", tile);
                }
                roomText += "\n";
            }
            System.IO.File.WriteAllText(@"RoomsAndTiles.txt", roomText);

        }
        private void TestBorders()
        {
            string roomText = "";
            foreach (FloorSet f in IndividualRooms.Values)
            {
                roomText += ("Room with ID:" + f.getID() + "has following Borders ---");

                //Console.Write("Room with ID: {0}  has following tiles ---", f.getID());
                //System.IO.File.WriteAllText(@"RoomsAndTiles.txt", roomText);
                foreach (var t in f.getBorders())
                {
                    roomText += "ID: " + t.Key + " Borders: ";
                    foreach(var b in t.Value)
                    {
                        roomText += b + "  ";
                    }
                }
                roomText += "\n";
            }
            System.IO.File.WriteAllText(@"Borders.txt", roomText);

        }
        //HElP METHODS
        private List<int[]> FloodFill(int ID, int X, int Y, int target, int[][] roomMap, bool corners)
        {
            //StopWatchStart();
            Dictionary<string,int[]> floodFiled = new Dictionary<string,int[]>();
            Queue<int[]> queue = new Queue<int[]>();
            queue.Enqueue(new int[] { X, Y });
            while (queue.Count() > 0)
            {
                if (!floodFiled.ContainsKey(X + "." + Y)) floodFiled.Add(X+"."+Y,new int[] { X, Y });
                int indexX = queue.Peek()[0];
                int indexY = queue.Dequeue()[1];
                int[] W = { indexX, indexY };
                int[] E = { indexX, indexY };

                while (W[1] >= 0 && W[1] < roomMap[1].Length && W[0] >= 0 && W[0] < roomMap[1].Length && roomMap[W[1]][W[0]] == target)
                {
                    W[0]--;
                }

                while (E[1] >= 0 && E[1] < roomMap[1].Length && E[0] >= 0 && E[0] < roomMap[1].Length && roomMap[E[1]][E[0]] == target)
                {
                    E[0]++;
                }
                for (int i = W[0] + 1; i < E[0]; i++)
                {
                    roomMap[W[1]][i] = ID;
                    if(!floodFiled.ContainsKey(i + "." + W[1]))floodFiled.Add(i +"."+ W[1], new int[] { i, W[1] });
                    if (W[1] + 1 >= 0 && W[1] + 1 < roomMap.Length && roomMap[W[1] + 1][i] == target)
                    {
                        queue.Enqueue(new int[] { i, W[1] + 1 });
                    }
                    if (corners)
                    {
                        if (W[1] + 1 >= 0 && W[1] + 1 < roomMap.Length && i + 1 < roomMap.Length && roomMap[W[1] + 1][i + 1] == target)
                        {
                            queue.Enqueue(new int[] { i + 1, W[1] + 1 });
                        }
                        if (W[1] - 1 >= 0 && W[1] - 1 < roomMap.Length && i + 1 < roomMap.Length && roomMap[W[1] - 1][i + 1] == target)
                        {
                            queue.Enqueue(new int[] { i + 1, W[1] - 1 });
                        }
                        if (W[1] + 1 >= 0 && W[1] + 1 < roomMap.Length && i - 1 >= 0 && roomMap[W[1] + 1][i - 1] == target)
                        {
                            queue.Enqueue(new int[] { i - 1, W[1] + 1 });
                        }
                        if (W[1] - 1 >= 0 && W[1] - 1 < roomMap.Length && i - 1 >= 0 && roomMap[W[1] - 1][i - 1] == target)
                        {
                            queue.Enqueue(new int[] { i - 1, W[1] - 1 });
                        }
                    }

                    if (W[1] - 1 >= 0 && W[1] - 1 < roomMap.Length && roomMap[W[1] - 1][i] == target)
                    {
                        queue.Enqueue(new int[] { i, W[1] - 1 });
                    }
                }
            }
            //StopWatchStop("FloodFillCorners");
            return floodFiled.Values.ToList();
        }
        private void DeleteSingles()
        {
            StopWatchStart();
            int[][] shifts =
            {
                new int[]{0, 1},
                new int[]{0,-1},
                new int[]{1,0},
                new int[]{-1,0}
            };
            for(int h = 1; h< Height-1; h++)
            {
                for(int w = 1; w< Width-1; w++)
                {
                    bool delete = true;
                    for(int i = 0; i< shifts.Length; i++)
                    {
                        if (Grid[h + shifts[i][1]][w + shifts[i][0]] == 0)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if (delete)
                    {
                        Grid[h][w] = Grid[h][w + 1];
                    }
                }

            }
            StopWatchStop("DeleteSingles");

        }
        private void MapBorders(Dictionary<int, FloorSet> FloorSetDictionary, int[][] Grid, int Target)
        {
            int[][] indexes =
            {
                new int[]{0,1,0,-1 },
                new int[]{1,0,-1,0 },
            };
            //Search trohrou the borders and look for WALL tile that conect 2 diferent FloorSet
            for(int i = 1; i< Grid.Length-1; i++)
            {
                for(int j = 1; j<Grid[0].Length-1; j++)
                {
                    if (Grid[i][j] == Target)
                    {
                        for(int l = 0; l<indexes.Length; l++)
                        {
                            Console.WriteLine("SearchingBorder");
                            int iX1 = j + indexes[l][0];
                            int iY1 = i + indexes[l][1];
                            int iX2 = j + indexes[l][2];
                            int iY2 = i + indexes[l][3];
                            if (iX1 <= 0 || iX2 <= 0 || iY1 <= 0 || iY2 <= 0 || iX1 >= Width || iX2 >= Width || iY1 >= Height || iY2 >= Height)
                            {
                                Console.WriteLine("OutOfBounds Coordinates 1: {0}  Coordinates 2: {1} ", iX1 +"." + iY1, iX2 + "." + iY2);
                                continue;
                            }
                            if (Grid[iY1][iX1] != Grid[iY2][iX2] && Grid[iY1][iX1] >Reserved && Grid[iY2][iX2] > Reserved)
                            {
                                FloorSetDictionary[Grid[iY1][iX1]].addBorder(Grid[iY2][iX2], i + "." + j);
                                FloorSetDictionary[Grid[iY2][iX2]].addBorder(Grid[iY1][iX1], i + "." + j);
                                break;
                            }
                        }

                    }
                }
            }

        }


    }
}
