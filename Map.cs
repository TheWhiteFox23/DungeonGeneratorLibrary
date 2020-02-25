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
        readonly private int BORDER = 0;
        readonly private int UNMARKEDFLOOR = 1;
        readonly private int WALL = 2;
        readonly private int DOOR = 3;
        readonly private int CENTER = 4;
        readonly private int VALIDSTART = 5;
        readonly private int INVALIDSTART = 6;
        readonly private int MAPPED = 7;
        readonly private int TEMPORARY = 8;



        public Map(int Width, int Height)
        {
            this.Height = Height;
            this.Width = Width;
            Grid = new int[Height][];
            random = new Random(seed);
            OnCreate();
        }

        /// <summary>
        /// Gather all the methods necessary for succesful cretion of the MAP class instance
        /// </summary>
        private void OnCreate()
        {
            InitializeGrid();
        }


        /// <summary>
        /// Initialize 2D int array representing the MAP
        /// </summary>
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


        /// <summary>
        /// Fill empty grid fith rooms of random size (Within the range) - rooms are tightly packed together
        /// </summary>
        /// <param name="MinRoomSize">Minimum size of the rooms</param>
        /// <param name="MaxRoomSize">Masimum size of the rooms</param>
        public void GenerateRoomTightSquares(int MinRoomSize, int MaxRoomSize)
        {
            StopWatchStart();
            //Dictionary<string, int[]> ValidStarts = new Dictionary<string, int[]>();
            //find all valid starts
            int[][] GridCopy = new int[Grid.Length][];
            Array.Copy(Grid, 0, GridCopy, 0, Grid.Length);
            for(int i = 1; i<Height-1; i++)
            {
                for(int j = 1; j< Width-1; j++)
                {
                    //ValidStarts.Add(i + "." + j, new int[] { i, j});
                    GridCopy[i][j] = VALIDSTART;
                }
            }
            //Console.WriteLine("\t Valid StartFund ---- ElapsedTime :  {0}", watch.Elapsed);

            //Filling with rooms
            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width- 1; j++)
                {
                    if (GridCopy[i][j] == VALIDSTART)
                    {
                        int H = random.Next(MinRoomSize, MaxRoomSize);
                        int W = random.Next(MinRoomSize, MaxRoomSize);
                        for(int a = 0; a < H; a++)
                        {
                            for(int b = 0; b< W; b++)
                            {
                                int indexX = j + b;
                                int indexY = i + a;
                                if (indexX > 0 && indexY > 0 && indexX < Width - 1 && indexY < Height - 1 && GridCopy[indexY][indexX] == VALIDSTART)
                                {
                                    GridCopy[indexY][indexX] = INVALIDSTART;
                                    //ValidStarts.Remove(indexY + "."  + indexX);
                                    Grid[indexY][indexX] = UNMARKEDFLOOR;
                                }
                            }
                        }
                        for(int a = j-1; a<= j + W; a++)
                        {
                            int iX = a;
                            int iY = (i - 1);
                            string border = iY + "." + iX;
                            if (iX > 0 && iY > 0 && iX < Width - 1 && iY < Height - 1 && GridCopy[iY][iX] == VALIDSTART)
                            {
                                GridCopy[iY][iX] = INVALIDSTART;
                                //ValidStarts.Remove(border);
                                Grid[iY][iX] = WALL;
                            }

                        }
                        for (int a = j - 1; a <=j + W; a++)
                        {
                            int iX = a;
                            int iY = (i + H);
                            string border = iY + "." + iX;
                            if (iX > 0 && iY > 0 && iX < Width - 1 && iY < Height - 1 && GridCopy[iY][iX] == VALIDSTART)
                            {
                                GridCopy[iY][iX] = INVALIDSTART;
                                Grid[iY][iX] = WALL;
                            }
                        }
                        for (int a = i - 1; a <= i +H ; a++)
                        {
                            int iX = j-1;
                            int iY = (a);
                            string border = iY + "." + iX;
                            if (iX > 0 && iY > 0 && iX < Width - 1 && iY < Height - 1 && GridCopy[iY][iX] == VALIDSTART)
                            {
                                GridCopy[iY][iX] = INVALIDSTART;
                                Grid[iY][iX] = WALL;
                            }
                        }
                        for (int a = i - 1; a <=i+ H; a++)
                        {
                            int iX = j+W;
                            int iY = a;
                            string border = iY + "." + iX;
                            if (iX > 0 && iY > 0 && iX < Width - 1 && iY < Height - 1 && GridCopy[iY][iX] == VALIDSTART)
                            {
                                GridCopy[iY][iX] = INVALIDSTART;
                                Grid[iY][iX] = WALL;
                            }
                        }
                    }
                }
            }
            StopWatchStop("GenerateRoomTightSquares");
        }


        /// <summary>
        /// Create image file representing the MAP
        /// </summary>
        public void BufferImage()
        {
            StopWatchStart();
            if (generateImage)
            {
                Dictionary<int, byte[]> ColorPallete = new Dictionary<int, byte[]>();
                ColorPallete.Add(BORDER, new byte[] { 0, 0, 0 });
                ColorPallete.Add(WALL, new byte[] { 255, 255, 255 });
                ColorPallete.Add(DOOR, new byte[] { 125, 125, 125 });
                ColorPallete.Add(MAPPED, new byte[] { 125, 125, 125 });
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


        /// <summary>
        /// Print CHAR representation of the map on the console(Unreadable for large outputs)
        /// </summary>
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


        /// <summary>
        ///         /// Conectint (Create DOOR tile in the wall) all the rooms in the MAP grid
        /// </summary>
        /// <param name="FinalID">INT - tiles will be set to this Value</param>
        /// <param name="TargetID">Target value - this tiles will be replaced with FinalID</param>
        public void ConectRoomTightSquaresRAMOptimalized(int FinalID, int TargetID)
        {
            StopWatchStart();
            FloodFillConnect(FinalID,TargetID, 1, 1, Grid);
            for(int i = 1; i<Height; i++)
            {
                for(int j = 1; j<Width; j++)
                {
                    if (Grid[i][j] == UNMARKEDFLOOR)
                    {
                        List<int[]> Borders = FloodFillBorderMaping(TEMPORARY, j, i, UNMARKEDFLOOR, Grid);
                        Dictionary<string, int[]> ValidDoorPositionsAndFirsNeighbourtTile = new Dictionary<string, int[]>();
                        MapBorders(ValidDoorPositionsAndFirsNeighbourtTile, Borders, WALL, Grid, TEMPORARY, MAPPED);
                        int[] Coordinates = new int[2];
                        if (ValidDoorPositionsAndFirsNeighbourtTile.Count > 0)
                        {
                            Coordinates = ValidDoorPositionsAndFirsNeighbourtTile.Values.ElementAt(random.Next(ValidDoorPositionsAndFirsNeighbourtTile.Count));
                        }
                        else
                        {
                            break;
                        }
                        FloodFill(UNMARKEDFLOOR, Coordinates[0], Coordinates[1], MAPPED, Grid, false);
                        FloodFill(UNMARKEDFLOOR, j, i, TEMPORARY, Grid, false);
                        FloodFillConnect(FinalID, TargetID, Coordinates[0], Coordinates[1], Grid);
                    }
                }
            }
            StopWatchStop("ConectRoomTightSquaresRAMOptimalized");
        }


        /// <summary>
        /// Start watch - Used for debuging purpose - enabled/disabled by setDebugging(bool setting)
        /// </summary>
        private void StopWatchStart()
        {
            if (diagnostics)
            {
                watch.Start();
            }
        }
        /// <summary>
        /// Restart watch and print elapset time, together with name of the tested method - Used for debuging purpose - enabled/disabled by setDebugging(bool setting)
        /// </summary>
        /// <param name="testedProcedure">Name of the tested method</param>
        private void StopWatchRestart(string testedProcedure)
        {
            if (diagnostics)
            {
                Console.WriteLine("Procedure --- {0} --- finished execution in time : {1}", testedProcedure, watch.Elapsed);
                watch.Restart();
            }
        }
        /// <summary>
        /// Stop watch and print elapset time, together with name of the tested method - Used for debuging purpose - enabled/disabled by setDebugging(bool setting)
        /// </summary>
        /// <param name="testedProcedure">Name of the tested method</param>
        private void StopWatchStop(string testedProcedure)
        {
            if (diagnostics)
            {
                Console.WriteLine("Procedure --- {0} --- finished execution in time : {1}", testedProcedure, watch.Elapsed);
                watch.Reset();
                watch.Stop();
            }
        }

        /// <summary>
        /// Floodfill continous tile in the grid - retun list (if the int[2]), representing X and Y coordinates of the FloodFiled tiles of the MAP grid
        /// </summary>
        /// <param name="ID">INT - Continuous tiles will be set to this value</param>
        /// <param name="X"> X coordinate of the point, where floodFiling will start</param>
        /// <param name="Y"> Y coordinate of the point, where floodFiling will start</param>
        /// <param name="target">INT representing the type of the tile that should be replaced with ID</param>
        /// <param name="roomMap">2D where that will be effected by the FloodFill methods</param>
        /// <param name="corners">if TRUE corners will be count into the FloodFill</param>
        /// <returns></returns>

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
        /// <summary>
        /// FloodFill Continuous tiles and return List<int[2]> of the X and Y coordinates of the BORDER/WAll tiles 
        /// This method is currently used only in ConectRoomTightSquaresRAMOptimalized to get all of the BORDER tiles of the continuous TILES
        /// </summary>
        /// <param name="ID">INT - Continuous tiles will be replaced with this value</param>
        /// <param name="X">X coordinate of the point, where floodFiling will start</param>
        /// <param name="Y">Y coordinate of the point, where floodFiling will start</param>
        /// <param name="target">INT representing the type of the tile that should be replaced with ID</param>
        /// <param name="roomMap">2D where that will be effected by the FloodFill methods</param>
        /// <returns></returns>
        private List<int[]> FloodFillBorderMaping(int ID, int X, int Y, int target, int[][] roomMap)
        {
            //StopWatchStart();
            Dictionary<string,int[]> floodFiled = new Dictionary<string,int[]>();
            Dictionary<string, int[]> Borders = new Dictionary<string, int[]>();
            Queue<int[]> queue = new Queue<int[]>();
            queue.Enqueue(new int[] { X, Y });
            while (queue.Count() > 0)
            {
                if (!floodFiled.ContainsKey(X + "." + Y)) floodFiled.Add(X+"."+Y,new int[] { X, Y });
                int indexX = queue.Peek()[0];
                int indexY = queue.Dequeue()[1];
                int[] W = { indexX, indexY };
                int[] E = { indexX, indexY };


                //Increase WEST until WALL is found
                while (W[1] >= 0 && W[1] < roomMap[1].Length && W[0] >= 0 && W[0] < roomMap[1].Length && roomMap[W[1]][W[0]] == target)
                {
                    W[0]--;
                }
                //WEST is now coordinates of the WALL
                if(!Borders.ContainsKey(W[0] + "." + W[1]) && roomMap[W[1]][W[0]] == WALL)
                    Borders.Add(W[0] + "." + W[1], new int[] { W[0], W[1] });

                //Increase East till EAST is WAll
                while (E[1] >= 0 && E[1] < roomMap[1].Length && E[0] >= 0 && E[0] < roomMap[1].Length && roomMap[E[1]][E[0]] == target)
                {
                    E[0]++;
                }
                //EAST is now WAll and can be added into the Bolders
                if (!Borders.ContainsKey(E[0] + "." + E[1]) && roomMap[E[1]][E[0]] == WALL)
                    Borders.Add(E[0] + "." + E[1], new int[] { E[0], E[1] });

                for (int i = W[0] + 1; i < E[0]; i++)
                {
                    roomMap[W[1]][i] = ID;
                    if(!floodFiled.ContainsKey(i + "." + W[1]))floodFiled.Add(i +"."+ W[1], new int[] { i, W[1] });
                    if (W[1] + 1 < roomMap.Length && roomMap[W[1] + 1][i] == target)
                    {
                        queue.Enqueue(new int[] { i, W[1] + 1 });
                    }else if(W[1] + 1 < roomMap.Length && roomMap[W[1] + 1][i] == WALL)
                    {
                        if (!Borders.ContainsKey(i + "." + (W[1] + 1)))
                            Borders.Add(i + "." + (W[1]+1), new int[] { i, W[1]+1 });
                    }
                    if (W[1] - 1 >= 0 && roomMap[W[1] - 1][i] == target)
                    {
                        queue.Enqueue(new int[] { i, W[1] - 1 });
                    }
                    else if (W[1] - 1 >= 0 && roomMap[W[1] - 1][i] == WALL)
                    {
                        if (!Borders.ContainsKey(i + "." + (W[1] - 1)))
                            Borders.Add(i + "." + (W[1] - 1), new int[] { i, W[1] - 1 });
                    }
                }
            }
            //StopWatchStop("FloodFillCorners");
            return Borders.Values.ToList();
        }
        /// <summary>
        /// MAP borders of the continuous tile set(presenting to the method in ListOfBlockToCheck parameter)
        /// check if the parameter of the border tiles correspods with the setting -  IF inner and outer tilese is same as presented in parameters
        /// Write results in given dictionary 
        /// </summary>
        /// <param name="ValidDoorPositionsAndFirsNeighbourtTile">Dictionary where valid border tiles will be stored in format:
        /// KEY - string YCoordinate.XCoordinate int[]{Xcoordinate, YCoordinate}</param>
        /// <param name="ListOfBlockToCheck">List of tested border tiles</param>
        /// <param name="Target">Tipe of the border tile Ussualy WALL</param>
        /// <param name="Grid">2D where will method look for ansvers</param>
        /// <param name="InnerCell">Inner cell type</param>
        /// <param name="OuterCell">Outer cell type</param>
        private void MapBorders(Dictionary<string, int[]> ValidDoorPositionsAndFirsNeighbourtTile, List<int[]> ListOfBlockToCheck, int Target, int[][] Grid, int InnerCell, int OuterCell)
        {
            //StopWatchStart();
            int[][] indexes =
            {
                new int[]{0,1,0,-1 },
                new int[]{1,0,-1,0 },
            };
            //Search trohrou the borders and look for WALL tile that conect 2 diferent FloorSet
             foreach(var i in ListOfBlockToCheck)
             {
                 if (Grid[i[1]][i[0]] == Target)
                 {
                     for(int l = 0; l<indexes.Length; l++)
                     {
                            //Console.WriteLine("SearchingBorder");
                         int iX1 = i[0] + indexes[l][0];
                         int iY1 = i[1] + indexes[l][1];
                         int iX2 = i[0] + indexes[l][2];
                         int iY2 = i[1] + indexes[l][3];
                        //Console.Write(iX1 + "." + iY1 + "-----" + iX2 + "." + iY2 + "");
                         if (iX1 <= 0 || iX2 <= 0 || iY1 <= 0 || iY2 <= 0 || iX1 >= Width || iX2 >= Width || iY1 >= Height || iY2 >= Height)
                         {
                             //Console.WriteLine("OutOfBounds Coordinates 1: {0}  Coordinates 2: {1} ", iX1 +"." + iY1, iX2 + "." + iY2);
                             continue;
                            }
                        if (Grid[iY1][iX1] == InnerCell && Grid[iY2][iX2] == OuterCell)
                        {
                            //Console.WriteLine("Found");
                            ValidDoorPositionsAndFirsNeighbourtTile.Add(i[1] + "." + i[0], new int[] { iX2, iY2 });
                            break;
                        }
                        else if (Grid[iY2][iX2] == InnerCell && Grid[iY1][iX1] == OuterCell)
                        {
                            //Console.WriteLine("Found");
                            ValidDoorPositionsAndFirsNeighbourtTile.Add(i[1] + "." + i[0], new int[] { iX1, iY1 });
                            break;
                        }
                     }
                    //Console.WriteLine();
                 }
             }
            
            //StopWatchStop("MapingBorders");

        }
        /// <summary>
        /// Take string in format YCoordinate.Xcoordinate and return int[]{XCoordinate, YCoordinate}
        /// </summary>
        /// <param name="mapOutput">Inpurt string in format YCoordinate.Xcoordinate</param>
        /// <returns></returns>
        private int[] ParseMap(string mapOutput)
        {
            string X = "";
            string Y = "";
            bool dot = false;
            var arr = mapOutput.ToCharArray();
            for (int i = 0; i < mapOutput.Length; i++)
            {
                if (!dot && arr[i] != '.')
                {
                    Y = Y + arr[i];
                }
                else if (arr[i] == '.')
                {
                    dot = true;
                }
                else
                {
                    X = X + arr[i];
                }
            }
            int[] ret = { int.Parse(X), int.Parse(Y) };
            return ret;

        }
        /// <summary>
        /// Help method for ConectRoomTightSquaresRAMOptimalized - randomly conect stream of tiles with given ID
        /// </summary>
        /// <param name="ID">ID - RoomTiles will be set to this type</param>
        /// <param name="TARGET">Type of the tile that will be replaced</param>
        /// <param name="CoordinateX">XCoordinate of the point inside the inicial room</param>
        /// <param name="CoordinateY">YCoordinate of the point inside the inicial room</param>
        /// <param name="Grid">2D array where changes will take place</param>
        private void FloodFillConnect(int ID,int TARGET, int CoordinateX, int CoordinateY, int[][] Grid)
        {
            while (true)
            {
                List<int[]> Borders = FloodFillBorderMaping(ID, CoordinateX, CoordinateY, TARGET, Grid);
                Dictionary<string, int[]> ValidDoorPositionsAndFirsNeighbourtTile = new Dictionary<string, int[]>();
                MapBorders(ValidDoorPositionsAndFirsNeighbourtTile, Borders, WALL, Grid, ID, TARGET);
                if (ValidDoorPositionsAndFirsNeighbourtTile.Count == 0) break;

                //randomly choose one valid border
                string RandomBorder = ValidDoorPositionsAndFirsNeighbourtTile.Keys.ElementAt(random.Next(ValidDoorPositionsAndFirsNeighbourtTile.Count));
                int[] Coordinates = ParseMap(RandomBorder);
                Grid[Coordinates[1]][Coordinates[0]] = DOOR;
                Coordinates = ValidDoorPositionsAndFirsNeighbourtTile[RandomBorder];
                CoordinateX = Coordinates[0];
                CoordinateY = Coordinates[1];
            }
        }


        
        





        //REMOVE OR REWRITE
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
            for (int h = 1; h < Height - 1; h++)
            {
                for (int w = 1; w < Width - 1; w++)
                {
                    bool delete = true;
                    for (int i = 0; i < shifts.Length; i++)
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
                    foreach (var b in t.Value)
                    {
                        roomText += b + "  ";
                    }
                }
                roomText += "\n";
            }
            System.IO.File.WriteAllText(@"Borders.txt", roomText);

        }
        public void ConectRoomTightSquaresRAMOptimalized_OLD()
        {
            StopWatchStart();
            /*1. Loop Throu Grid and find rooms - if unconected - FloodfileRoom
             * 2. Map rooms Border
             * 3. repleca one border by the door
             * 4. set neighbourt room filed and continuet the loop
             */
            int[][] GridCopy = new int[Grid.Length][];
            Array.Copy(Grid, 0, GridCopy, 0, Grid.Length);
            FloodFill(MAPPED, 1, 1, UNMARKEDFLOOR, GridCopy, false);
            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    if (GridCopy[i][j] == UNMARKEDFLOOR)
                    {
                        var list = FloodFillBorderMaping(MAPPED, j, i, 1, GridCopy);
                        Dictionary<string, int[]> Borders = new Dictionary<string, int[]>();
                        MapBorders(Borders, list, WALL, GridCopy, MAPPED, UNMARKEDFLOOR);
                        /*foreach(var r in Borders)
                        {
                            Console.Write(r.Key + "  ");
                        }
                        Console.WriteLine();*/
                        if (Borders.Count > 0)
                        {
                            string RandomBorderTile = Borders.Keys.ElementAt(random.Next(Borders.Count));
                            int[] Doors = ParseMap(RandomBorderTile);
                            Grid[Doors[1]][Doors[0]] = DOOR;
                            FloodFill(MAPPED, Borders[RandomBorderTile][0], Borders[RandomBorderTile][1], UNMARKEDFLOOR, GridCopy, false);
                            //BufferImage();
                            //Console.ReadKey();
                        }
                        /*int[] Doors = ParseMap(RandomBorderTile);
                        Grid[Doors[1]][Doors[0]] = DOOR;
                        FloodFill(MAPPED, Borders[RandomBorderTile][0], Borders[RandomBorderTile][1], UNMARKEDFLOOR, GridCopy, false);
                        */
                        /*Console.Write("Current Borders");
                        foreach(var r in list)
                        {
                            Console.Write(r[0] + "." + r[1] + "  ");
                        }
                        Console.WriteLine();*/
                    }
                }
            }

            StopWatchStop("ConectRoomTightSquaresRAMOptimalized");
        }
        public void ConectRoomTightSquares()
        {
            StopWatchStart();
            Dictionary<int, FloorSet> IndividualRooms = new Dictionary<int, FloorSet>();
            MapRooms(IndividualRooms);
            MapBorders(IndividualRooms, Grid, WALL);
            Dictionary<int, int> ConectedRooms = new Dictionary<int, int>();
            Dictionary<int, List<string>> PossibleConnections = new Dictionary<int, List<string>>();

            /* 0. create distionyry of conected rooms, and distionary of posible conections
             * 1. randomly select room - add to diactionary
             * 2.get neughbourts - add to dictionary
             * 3 select one neighbort and one border tile
             * 4. replace with door tile and continue untill all tiles are conected*/

            //Select fors room and add to dictionary
            FloorSet FirtRoom = IndividualRooms.Values.ElementAt(random.Next(0, IndividualRooms.Count()));
            ConectedRooms.Add(FirtRoom.getID(), 1);
            PossibleConnections = FirtRoom.getBorders();

            while (ConectedRooms.Count != IndividualRooms.Count)
            {
                int IDOfTheNextRoom = PossibleConnections.Keys.ElementAt(random.Next(PossibleConnections.Count));
                int[] CoordinatesOfTheBorderTile = ParseMap(PossibleConnections[IDOfTheNextRoom].ElementAt(random.Next(PossibleConnections[IDOfTheNextRoom].Count)));
                Grid[CoordinatesOfTheBorderTile[1]][CoordinatesOfTheBorderTile[0]] = DOOR;

                ConectedRooms.Add(IDOfTheNextRoom, 1);
                Dictionary<int, List<string>> ToMerge = IndividualRooms[IDOfTheNextRoom].getBorders();

                //Merging Posible Conections
                foreach (var i in ToMerge)
                {
                    if (!ConectedRooms.ContainsKey(i.Key) && PossibleConnections.ContainsKey(i.Key))
                    {
                        List<string> Merge = i.Value;
                        foreach (var j in Merge)
                        {
                            if (!PossibleConnections[i.Key].Contains(j))
                            {
                                PossibleConnections[i.Key].Add(j);
                            }
                        }
                    }
                    else if (!ConectedRooms.ContainsKey(i.Key))
                    {
                        PossibleConnections.Add(i.Key, i.Value);
                    }
                }

                PossibleConnections.Remove(IDOfTheNextRoom);
            }
            StopWatchStop("ConectRoomTightSquares");
        }
        private void MapRooms()
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
                        FloorSet newFloorSet = new FloorSet(FloodFill(ID, j, i, 1, Grid, false), ID);
                        //FloodFill(ID, j, i, 1, Grid, false);
                        IndividualRooms.Add(ID, newFloorSet);
                        ID++;
                    }

                }
            }
            StopWatchStop("MapRooms");
        }
        private void MapRooms(Dictionary<int, FloorSet> IndividualRooms)
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
                        FloorSet newFloorSet = new FloorSet(FloodFill(ID, j, i, 1, Grid, false), ID);
                        //FloodFill(ID, j, i, 1, Grid, false);
                        IndividualRooms.Add(ID, newFloorSet);
                        ID++;
                    }

                }
            }
            StopWatchStop("MapRooms");
        }
        private void MapRooms(int MinSize)
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
                            foreach (var f in FloorTiles)
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
        private void MapRoomsFixedID(int FixedID)
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
        private void DeleteInnerBlockWalls()
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
        } //Obsolent
        private void GenerateRoomsRandom()
        {
            StopWatchStart();
            //Random random = new Random(seed);
            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    Grid[i][j] = random.Next(0, 2);
                }
            }
            StopWatchStop("GenerateRoomsRandom");

        } //Only Test - Cannod be controled - replace with more variable option
        private void MapBorders(Dictionary<int, FloorSet> FloorSetDictionary, int[][] Grid, int Target)

        {
            StopWatchStart();
            int[][] indexes =
            {
                new int[]{0,1,0,-1 },
                new int[]{1,0,-1,0 },
            };
            //Search trohrou the borders and look for WALL tile that conect 2 diferent FloorSet
            for (int i = 1; i < Grid.Length - 1; i++)
            {
                for (int j = 1; j < Grid[0].Length - 1; j++)
                {
                    if (Grid[i][j] == Target)
                    {
                        for (int l = 0; l < indexes.Length; l++)
                        {
                            //Console.WriteLine("SearchingBorder");
                            int iX1 = j + indexes[l][0];
                            int iY1 = i + indexes[l][1];
                            int iX2 = j + indexes[l][2];
                            int iY2 = i + indexes[l][3];
                            if (iX1 <= 0 || iX2 <= 0 || iY1 <= 0 || iY2 <= 0 || iX1 >= Width || iX2 >= Width || iY1 >= Height || iY2 >= Height)
                            {
                                //Console.WriteLine("OutOfBounds Coordinates 1: {0}  Coordinates 2: {1} ", iX1 +"." + iY1, iX2 + "." + iY2);
                                continue;
                            }
                            if (Grid[iY1][iX1] != Grid[iY2][iX2] && Grid[iY1][iX1] > Reserved && Grid[iY2][iX2] > Reserved)
                            {
                                FloorSetDictionary[Grid[iY1][iX1]].addBorder(Grid[iY2][iX2], i + "." + j);
                                FloorSetDictionary[Grid[iY2][iX2]].addBorder(Grid[iY1][iX1], i + "." + j);
                                break;
                            }
                        }

                    }
                }
            }
            StopWatchStop("MapingBorders");

        }
    }
}
