using System.Text;

internal class Program
{
    static string INPUT_DIRECTORY = "../../../input/";
    static string[] INPUT_FILES = { "00-trailer.txt" };
    static string OUTPUT_DIRECTORY = "../../../output/";
    static string[] OUTPUT_FILES = { "00-trailer.txt" };

    // Define variables to store the input values
    static int W, H, N, M, L;
    static List<Point> G = new List<Point>();
    static List<SilverPoint> S = new List<SilverPoint>();
    static Dictionary<Tiles, int> TilesAvailable = new Dictionary<Tiles, int>();
    static Dictionary<Tiles, int> TilesCost = new Dictionary<Tiles, int>();

    static Dictionary<Point, Tiles> TileUsate = new Dictionary<Point, Tiles>();
    static int punteggio = 0;


    private static void Main(string[] args)
    {
        Read();
        Solve();
        Write();

        CalculateLoadRoad();
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
                int TIDk = Encoding.ASCII.GetBytes(parts[0]).First();
                int TNk = int.Parse(parts[1]);
                int TCk = int.Parse(parts[2]);
                TilesAvailable.Add((Tiles) TIDk, TCk);
                TilesCost.Add((Tiles) TIDk, TNk);
            }
        }
    }
    static void Solve()
    {
        Dictionary<Tuple<Point, Point>, int> distanzeTraGoldenPoint = new Dictionary<Tuple<Point, Point>, int>();

        // calcoliamo le distanze
        for (int i = 0; i < N - 1; i++)
        {
            Point first = G.ElementAt(i);
            for (int j = i + 1; j < N; j++)
            {
                Point second = G.ElementAt(j);
                int distance = Math.Abs(first.x - second.x) + Math.Abs(first.y - second.y);
                distanzeTraGoldenPoint.Add(new Tuple<Point, Point>(first, second), distance);
            }
        }

        for ( int i = 0; i < distanzeTraGoldenPoint.Count; i++)
        {
            var closest = distanzeTraGoldenPoint.OrderBy(kvp => kvp.Value).First();

            //coprire il percorso tra closest.key.Item1 e closest.key.Item2
            try
            {
                foreach (SilverPoint silverPoint in S)
                {
                    FindTheTilesAmong(closest.Key.Item1, silverPoint);
                    if (punteggio > 0)
                    {
                        FindTheTilesAmong(silverPoint, closest.Key.Item2);
                        if (punteggio > 0)
                            break;
                    }
                }


            }
            catch (Exception e) { Console.WriteLine("Non ci sono abbastanza tile"); }
            distanzeTraGoldenPoint.Remove(closest.Key);
        }
    }

    private static void FindTheTilesAmong(Point G1, Point G2)
    {
        int tempPunteggio = 0;
        Dictionary<Tiles, int> tempTilesAvailable = new Dictionary<Tiles, int>();
        foreach (var tile in TilesAvailable.Keys)
        {
            tempTilesAvailable.Add(tile, TilesAvailable[tile]);
        }
        Dictionary<Point, Tiles> tempTileUsate = new Dictionary<Point, Tiles>();

        Point currentPosition = G1;
        while (currentPosition.x != G2.x) //se mi devo spostare in x
        {
            int spostamentoX = (currentPosition.x < G2.x) ? 1 : -1;
            List<Tiles> disponibili = new List<Tiles> { Tiles.rettilineo_orizzontale, Tiles.incrocio, Tiles.t_sinistra_destra_su, Tiles.t_sinistra_destra_giu };
            Tiles? menoCostosa = disponibili.Where(t => tempTilesAvailable[t] > 0).OrderBy(t => TilesCost[t]).FirstOrDefault();
            if (menoCostosa == null) throw new Exception();

            currentPosition = new Point(currentPosition.x + spostamentoX, currentPosition.y);
            KeyValuePair<Point, Tiles>? pastTile = TileUsate.Where(t => t.Key.x == currentPosition.x && t.Key.y == currentPosition.y).FirstOrDefault();
            int currentPoint = S.Where(s => s.x == currentPosition.x && s.y == currentPosition.y).FirstOrDefault()?.score ?? 0;
            if (currentPoint != 0)
            {
                tempPunteggio += currentPoint;
            }
            else if (currentPosition.y != G2.y || currentPosition.x != G2.x)
            {
                if (pastTile == null)
                {
                    tempTileUsate.Add(currentPosition, menoCostosa.Value);
                    tempTilesAvailable[menoCostosa.Value] = tempTilesAvailable[menoCostosa.Value] - 1;
                    tempPunteggio -= TilesCost[menoCostosa.Value];
                } else
                {
                    tempPunteggio -= TilesCost[pastTile.Value.Value];
                }
            }
        }
        while (currentPosition.y != G2.y) //se mi devo spostare in y
        {
            int spostamentoY = (currentPosition.y < G2.y) ? 1 : -1;
            List<Tiles> disponibili = new List<Tiles> { Tiles.rettilineo_verticale, Tiles.incrocio, Tiles.t_su_giu_sinistra, Tiles.t_su_giu_destra };
            Tiles? menoCostosa = disponibili.Where(t => tempTilesAvailable[t] > 0).OrderBy(t => TilesCost[t]).FirstOrDefault();
            if (menoCostosa == null) throw new Exception();

            currentPosition = new Point(currentPosition.x, currentPosition.y + spostamentoY);
            KeyValuePair<Point, Tiles>? pastTile = TileUsate.Where(t => t.Key.x == currentPosition.x && t.Key.y == currentPosition.y).FirstOrDefault();
            int currentPoint = S.Where(s => s.x == currentPosition.x && s.y == currentPosition.y).FirstOrDefault()?.score ?? 0;
            if (currentPoint != 0)
            {
                tempPunteggio += currentPoint;
            }
            else if (currentPosition.y != G2.y || currentPosition.x != G2.x)
            {
                if (pastTile == null)
                {
                    tempTileUsate.Add(currentPosition, menoCostosa.Value);
                    tempTilesAvailable[menoCostosa.Value] = tempTilesAvailable[menoCostosa.Value] - 1;
                    tempPunteggio -= TilesCost[menoCostosa.Value];
                } else
                {
                    tempPunteggio -= TilesCost[pastTile.Value.Value];
                }
            }
        }

        //riporta tutti i valori temporanei nelle variabili globali solo quando abbiamo raggiunto G2

        foreach (var tile in tempTilesAvailable.Keys)
        {
            TilesAvailable[tile] = tempTilesAvailable[tile];
        }

        foreach (var tile in tempTileUsate)
        {
            TileUsate.Add(tile.Key, tile.Value);
        }

        punteggio += tempPunteggio;

    }
    static void Write()
    {
        using (StreamWriter writer = File.CreateText(OUTPUT_DIRECTORY + OUTPUT_FILES[0]))
        {
            for(int i =0; i<TileUsate.Count; i++)
            {
                var item = TileUsate.ElementAt(i);
                writer.Write(Convert.ToChar((int)item.Value) + " " + item.Key.x + " " + item.Key.y);
                if (i < TileUsate.Count -1) writer.Write("\n");
            }
        }
    }

    static void CalculateLoadRoad()
    {
        Console.WriteLine("Punteggio " + punteggio);
    }

}

class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x; this.y = y;
    }
}

class SilverPoint : Point
{
    public int score;

    public SilverPoint(int x, int y, int score) : base(x, y)
    {
        this.score = score;
    }
}

enum Tiles
{
    rettilineo_orizzontale = 0x33,
    curva_giu_destra = 0x35,
    curva_sinistra_giu = 0x36,
    t_sinistra_destra_giu = 0x37,
    curva_su_destra = 0x39,
    curva_sinistra_su = 0x41,
    t_sinistra_destra_su = 0x42,
    rettilineo_verticale = 0x43,
    t_su_giu_destra = 0x44,
    t_su_giu_sinistra = 0x45,
    incrocio = 0x46
}