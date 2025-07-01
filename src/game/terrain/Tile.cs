
namespace Coreblock{

    public struct Tile {
        public int id;      // ID do bloco
        public int life;
        private readonly int tier;
        private readonly int dropId;
        public bool solid;  // É sólido?
        public bool propagate;
        public int propagateTileId;
        public int variant; // Carregado?
        public SlopeType slopeType; 
        public Tiles.TileType TileType;
        public LiquidType LiquidType;
        public byte LiquidLevel;

        public Tile(int id = 0, int life = 1, int tier = 1, int dropId = 0, bool solid = false, bool propagate = false, int propagateTileId = 0, SlopeType slopeType = SlopeType.None, Tiles.TileType tileType = Tiles.TileType.ar, LiquidType liquidType = LiquidType.None)
        {
            this.id = id;
            this.life = life;
            this.tier = tier;
            this.dropId = dropId;
            this.solid = solid;
            this.propagate = propagate;
            this.propagateTileId = propagateTileId;
            this.slopeType = slopeType;
            this.TileType = tileType;
            this.LiquidType = liquidType;
        }
    }
}