internal class Program
{
    static string INPUT_DIRECTORY = "../../../input/";
    static string[] INPUT_FILES = { "00-trailer.txt" };

    // Define variables to store the input values
    static int W, H, N, M, L;
    static List<Point> G = new List<Point>();
    static List<SilverPoint> S = new List<SilverPoint>();
    static Dictionary<Tile, int> TilesAvailable = new Dictionary<Tile, int>();
    static Dictionary<Tile, int> TilesCost = new Dictionary<Tile, int>();

    static Dictionary<Point, Tile> TileUsate = new Dictionary<Point, Tile>();
    private static void Main(string[] args)
    {
        Read();
        Solve();
        Write();
    }

    static void Read()
    {
        // Read the input file
        using (StreamReader reader = new StreamReader(INPUT_DIRECTORY + INPUT_FILES[0]))
        {
            // Read the first line containing the grid dimensions and point counts
            string[] row = reader.ReadLine().Split(' ');

            // Parse the values from the first line
            W = int.Parse(row[0]);
            H = int.Parse(row[1]);
            N = int.Parse(row[2]);
            M = int.Parse(row[3]);
            L = int.Parse(row[4]);

            for (int i = 0; i < N; i++)
            {
                row = reader.ReadLine().Split(' ');
                G.Add(new Point(int.Parse(row[0]), int.Parse(row[1])));
            }
            for (int i = 0; i < M; i++)
            {
                row = reader.ReadLine().Split(' ');
                S.Add(new SilverPoint(int.Parse(row[0]), int.Parse(row[1]), int.Parse(row[2])));
            }
            for (int i = 0; i < L; i++)
            {
                string[] parts = reader.ReadLine().Split(' ');
                string TIDk = parts[0];
                int TCk = int.Parse(parts[1]);
                int TNk = int.Parse(parts[2]);
                TilesAvailable.Add(Tile.fromCode(TIDk), TCk);
                TilesCost.Add(Tile.fromCode(TIDk), TNk);
            }
        }
    }
}

class Point
{
    int x;
    int y;

    public Point(int x, int y)
    {
        this.x = x; this.y = y;
    }
}

class SilverPoint: Point
{
    int score;

    public SilverPoint(int x, int y, int score) : base(x, y)
    {
        this.score = score;
    }
}

enum Tile
{
    rettilinio_orizzontale,
    svolta_giu_dx,
    svolta_sx_giu;
    //...

}