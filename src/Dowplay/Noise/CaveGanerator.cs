namespace Coreblock {
    public class CaveGanerator {
        public int width = 0;
        public int height = 0;
        public int smoothCycles;
        public int randFillPercent;    
        public int threshold;


        int[,] cavePoints;

        public CaveGanerator(int width, int height, int smoothCycles = 5, int randFillPercent = 53, int threshold= 4) {
            this.width = width;
            this.height = height;
            this.smoothCycles = smoothCycles;
            this.randFillPercent = randFillPercent;
            this.threshold = threshold;

            cavePoints = new int[width, height];
        }

        public int[,] generate(int seed){
            System.Random randChoice = new System.Random(seed.GetHashCode());

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if(x <= 5 || y <= 5 || x >= width - 15 || y >= height -5){
                        cavePoints[x, y] = 1;

                    }else if(randChoice.Next(0, 100) < randFillPercent){
                        cavePoints[x, y] = 1;

                    } else{
                        cavePoints[x, y] = 0;

                    }
                }
            }

            for(int i = 0; i < smoothCycles; i++){
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        int neighboringWalls = GetNeighbors(x, y);

                        if(neighboringWalls > threshold){
                            cavePoints[x, y] = 1;

                        }else if(neighboringWalls < threshold){
                            cavePoints[x, y] = 0;
                        }
                    }
                }
            }

            return cavePoints;
        }
        private int GetNeighbors(int pointX, int pointY){
            int wallNeighbors = 0;

            for (int x = pointX - 1; x <= pointX + 1; x++) {
                for (int y = pointY - 1; y <= pointY + 1; y++) {
                    
                    if(x >= 0 && x < width && y >= 0 && y < height){

                        if(x != pointX || y != pointY){
                            if(cavePoints[x, y] == 1){
                                wallNeighbors++;
                            }
                        }
                    }
                    else{
                        wallNeighbors++;
                    }
                }
            }

            return wallNeighbors;
        }
    }
}