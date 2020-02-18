using System;
using System.Collections.Generic;
using System.Linq;


namespace DungeonGenerator
{
    public class DungeonMap
    {
        readonly int X;
        readonly int Y;
        char[][] dMap;
        int [][] roomMap;
        char wall;
        char door;
        int maxRoomSize;
        int minRoomSize;
        Dictionary<string, int> validStarts;
        Dictionary<int, Room2> rooms2;
        System.Diagnostics.Stopwatch watch;
        Random rn = new Random();


        //CONSTRUCTOR
        public DungeonMap(int X, int Y, int maxRoomSize, int minRoomSize)
        {
            this.X = X;
            this.Y = Y;
            dMap = new char[Y][];
            roomMap = new int[Y][];
            this.maxRoomSize = maxRoomSize;
            this.minRoomSize = minRoomSize;
            wall = 'o';
            door = '!';
            validStarts = new Dictionary<string, int>();
            watch = new System.Diagnostics.Stopwatch();
            rooms2 = new Dictionary<int, Room2>();

            OnCreate();
        }
        private void OnCreate()
        {

            TimeOfExecutionStart(watch);
            FillMap(wall);
            TimeOfExecutionEnd(watch, "Filling the map");
            FindValid();
            TimeOfExecutionEnd(watch, "Finding Valid  ");
            GenerateRooms();
            TimeOfExecutionEnd(watch, "Generating rooms");
            MapRooms();
            TimeOfExecutionEnd(watch, "Maping Rooms   ");
            MapBorders2();
            TimeOfExecutionEnd(watch, "Maping Borders  ");
            ConectRooms2();
            //bufferRoomMap();
            TimeOfExecutionEnd(watch, "Conecting Rooms");
            watch.Stop();
            Console.WriteLine("Buffering Image");
            BufferRooms();
        }

        private void FillMap(char wall)
        {
            for (int i = 0; i < Y; i++)
            {
                char[] arr2 = new char[X];
                for (int j = 0; j < X; j++)
                {
                    arr2[j] = wall;
                }
                dMap[i] = arr2;
            }
        }

        private void GenerateRooms()
        {
            //Random rn = new Random();
            while (validStarts.Count != 0)
            {
                for (int i = 1; i < Y - 1; i++)
                {
                    for (int j = 1; j < X - 1; j++)
                    {
                        if (validStarts.ContainsKey(i + "." + j))
                        {
                            int roomSizeX = rn.Next(minRoomSize, maxRoomSize);
                            int roomSizeY = rn.Next(minRoomSize, maxRoomSize);
                            GenerateRoom(i + "." + j, roomSizeX, roomSizeY);

                        }

                    }
                }

            }
        }

        private void FindValid()
        {
            for (int i = 1; i < Y - 1; i++)
            {
                for (int j = 1; j < X - 1; j++)
                {
                    try
                    {
                        if (dMap[i - 1][j - 1] == wall &&
                            dMap[i - 1][j] == wall &&
                            dMap[i - 1][j + 1] == wall &&
                            dMap[i][j - 1] == wall &&
                            dMap[i][j + 1] == wall &&
                            dMap[i + 1][j - 1] == wall &&
                            dMap[i + 1][j] == wall &&
                            dMap[i + 1][j + 1] == wall)
                        {
                            validStarts.Add(i + "." + j, 1);
                            //dMap[i][j] = 'x';
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Invalid Index");
                    }
                }
            }
        }

        private void MapRooms()
        {
            //TODO - !!!!!!!room maping can be more effective - iterate throu raw map - in case current tile will be 1 - flood fill and save room


            /*new map room method - indexing similar then writing the indexes into room list
             *1. Create copy of the dMap byt in integers - walls are 0, floor has ID number of the room
             *2. Generate floor tiles based on id - if no negbourth tiles has id - create new ID else set id to the one of 
             * the neghbourt tile
             *3.create the rooms based on the id - borders can be simultinuosly maped with the rooms
             */
            FillRoomMap2();

            int ID = 2;
            //storing all tiles and indexes of the room
            Dictionary<string, int> allTiles = new Dictionary<string, int>();
            for (int i = 0; i < Y; i++)
            {
                for (int j = 0; j < X; j++)
                {
                    if (dMap[i][j] != 'o') allTiles.Add(i + "." + j, 0);

                }
            }

            while (allTiles.Count() > 0)
            {
                //randomly choose tile and perform flood fill
                var watch2 = new System.Diagnostics.Stopwatch();
                string toParse = allTiles.Keys.First();
                int indexX = ParseMap(toParse)[0];
                int indexY = ParseMap(toParse)[1];

                List<string> toRemove = FloodFill(ID, indexX, indexY, 1, roomMap);

                rooms2.Add(ID, new Room2(ID));

                ID++;

                foreach (var r in toRemove)
                {
                    allTiles.Remove(r);
                }

            }

        }

        private void MapBorders2()
        {
            int[][] indexes =
            {
                //indexes for X1
                new int[] {0,1},
                //indexes for Y1
                new int[] {1,0},
                //indexes for X2
                new int[] {0,-1},
                //indexes for Y2
                new int[] {-1,0}
            };
            for (int i = 1; i < Y - 1; i++)
            {
                for (int j = 1; j < X - 1; j++)
                {
                    if (roomMap[i][j] == 0)
                    {
                        for (int t = 0; t < indexes[0].Length; t++)
                        {
                            int indexX1 = j + indexes[0][t];
                            int indexY1 = i + indexes[1][t];
                            int indexX2 = j + indexes[2][t];
                            int indexY2 = i + indexes[3][t];
                            if (indexX1 >= X || indexX1 <= 0 || indexX2 >= X || indexX2 <= 0 || indexY1 >= Y || indexY1 <= 0 || indexY2 >= Y || indexY2 <= 0) continue;
                            if (roomMap[indexY1][indexX1] > 0 && roomMap[indexY2][indexX2] > 0 && roomMap[indexY1][indexX1] != roomMap[indexY2][indexX2])
                            {
                                if (!rooms2[roomMap[indexY1][indexX1]].GetBorder2().ContainsKey(i + "." + j)) rooms2[roomMap[indexY1][indexX1]].AddBorder2(i + "." + j, roomMap[indexY2][indexX2]);
                                if (!rooms2[roomMap[indexY2][indexX2]].GetBorder2().ContainsKey(i + "." + j)) rooms2[roomMap[indexY2][indexX2]].AddBorder2(i + "." + j, roomMap[indexY1][indexX1]);
                                break;
                            }
                        }

                    }
                }
            }
        }

        private void ConectRooms2()
        {
            /**Room Connect algorith destciption
             * 1, Select room - separate it from array
             * 2, Find all of the neghbourt of the room and randomly choose one
             * 3, selectst random border of the room - merge with current room and delete from list
             * 4, add current room to the list
             */
            //randomly choosing room
            Random random = new Random();
            int count = rooms2.Count();
;
            int rand = random.Next(2, count +1);
            Room2 megaRoom = rooms2[rand];


            while (rooms2.Count() != 1)
            {
                rooms2.Remove(megaRoom.GetID());
                //Choose one random neighbour and conect
                Room2 neighboutr = new Room2(); //empty constructor (ID is zero and all tiles are empty);
                int roomToChange = 0;

                if(megaRoom.GetSurrounding().Count != 0)
                {
                    roomToChange = megaRoom.GetSurrounding().First();
                }
                else
                {
                    Console.WriteLine("Error ocured during room maping");
                }

                string border = megaRoom.GetBorderMap()[roomToChange].Last();

                int coordinateX = ParseMap(border)[0];
                int coordinateY = ParseMap(border)[1];
                dMap[coordinateY][coordinateX] = door;
                roomMap[coordinateY][coordinateX] = int.MaxValue;

                //merging
                megaRoom.MergeWith2(rooms2[roomToChange], border);
                rooms2.Remove(roomToChange);
                rooms2.Add(megaRoom.GetID(), megaRoom);
            }
        }



        //HELP
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

        private void GenerateRoom(string seed, int Xsize, int Ysize)
        {
            int Xindex = ParseMap(seed)[0];
            int Yindex = ParseMap(seed)[1];
            List<string> room = new List<string>();
            Dictionary<string, int> mapBorder = new Dictionary<string, int>();


            //replace all valid within the range
            for (int i = 0; i < Ysize; i++)
            {
                for (int j = 0; j < Xsize; j++)
                {
                    int iX = Xindex + j;
                    int iY = Yindex + i;

                    if (validStarts.ContainsKey(iY + "." + iX))
                    {
                        room.Add(iY + "." + iX);
                        dMap[Yindex + i][Xindex + j] = ' ';
                        validStarts.Remove(iY + "." + iX);
                    }
                    else
                    {
                        break;
                    }

                }
            }

            for (int i = Xindex - 1; i < Xindex + Xsize + 1; i++)
            {
                int Yi = Yindex - 1;
                if (validStarts.ContainsKey(Yi + "." + i))
                {

                    dMap[Yi][i] = 'o';
                    validStarts.Remove(Yi + "." + i);
                }
            }

            for (int i = Xindex - 1; i < Xindex + Xsize + 1; i++)
            {
                int Yi = Yindex + Ysize;
                if (validStarts.ContainsKey(Yi + "." + i))
                {

                    dMap[Yi][i] = 'o';
                    validStarts.Remove(Yi + "." + i);

                }
            }

            for (int i = Yindex - 1; i < Yindex + Ysize + 1; i++)
            {
                int Xi = Xindex - 1;
                if (validStarts.ContainsKey(i + "." + Xi))
                {

                    dMap[i][Xi] = 'o';
                    validStarts.Remove(i + "." + Xi);
                }
            }

            for (int i = Yindex - 1; i < Yindex + Ysize + 1; i++)
            {
                int Xi = Xindex + Xsize;
                if (validStarts.ContainsKey(i + "." + Xi))
                {

                    dMap[i][Xi] = 'o';
                    validStarts.Remove(i + "." + Xi);

                }
            }
        }

        public void FillRoomMap2()
        {
            for (int i = 0; i < Y; i++)
            {
                roomMap[i] = new int[X];
                for (int j = 0; j < X; j++)
                {
                    roomMap[i][j] = 0;
                }
            }
            for (int i = 0; i < Y; i++)
            {
                for (int j = 0; j < X; j++)
                {
                    if (dMap[i][j] == 'o')
                    {
                        roomMap[i][j] = 0;
                    }
                    else
                    {
                        roomMap[i][j] = 1;
                    }
                }
            }
        }

        private List<string> FloodFill(int ID, int X, int Y, int target, int[][] roomMap)
        {
            List<string> floodFiled = new List<string>();
            List<string> borders = new List<string>();
            if (roomMap[Y][X] == ID)
            {
                Console.WriteLine("ID is already correct");
                return floodFiled;
            }
            else if (roomMap[Y][X] != target)
            {
                Console.WriteLine("ID do not response to target");
                return floodFiled;
            }

            Queue<int[]> queue = new Queue<int[]>();
            queue.Enqueue(new int[] { X, Y });
            while (queue.Count() > 0)
            {
                floodFiled.Add(Y + "." + X);
                int indexX = queue.Peek()[0];
                int indexY = queue.Dequeue()[1];
                int[] W = { indexX, indexY };
                int[] E = { indexX, indexY };

                while (roomMap[W[1]][W[0]] == target)
                {
                    W[0]--;
                }

                while (roomMap[E[1]][E[0]] == target)
                {
                    E[0]++;
                }
                for (int i = W[0] + 1; i < E[0]; i++)
                {
                    roomMap[W[1]][i] = ID;
                    floodFiled.Add(W[1] + "." + i);
                    if (roomMap[W[1] + 1][i] == target)
                    {
                        queue.Enqueue(new int[] { i, W[1] + 1 });
                    }
                    if (roomMap[W[1] - 1][i] == target)
                    {
                        queue.Enqueue(new int[] { i, W[1] - 1 });
                    }
                }
            }

            return floodFiled;
        }



        //DEBUG
        private void TimeOfExecutionStart(System.Diagnostics.Stopwatch watch)
        {
            watch.Reset();
            watch.Start();

        }

        private void TimeOfExecutionEnd(System.Diagnostics.Stopwatch watch, string message)
        {
            Console.WriteLine("\t {0} \t time of execution {1}", message, watch.Elapsed);
            watch.Reset();
            watch.Start();

        }

        private void BufferRooms()
        {
            ImageBuffer buffer = new ImageBuffer(X, Y);
            for(int i = 0; i< Y; i++)
            {
                for(int j = 0; j< X; j++)
                {
                    if (roomMap[i][j] == 0)
                    {
                        buffer.PlotPixel(j, i, 255, 255, 255);
                    }else if (roomMap[i][j] == int.MaxValue)
                    {
                        buffer.PlotPixel(j, i, 255, 0, 255);
                    }
                    else
                    {
                        buffer.PlotPixel(j, i, 0, 255, 0);
                    }
                }
            }
            buffer.save();
        }



        //PUBLIC
        public void Print()
        {
            for (int i = 0; i < Y; i++)
            {
                Console.Write("{0}: \t", i);
                for (int j = 0; j < X; j++)
                {
                    Console.Write("{0} ", dMap[i][j]);
                }
                Console.WriteLine();
            }
        }

        public char[][] GetMap()
        {
            return dMap;
        }


    }
}