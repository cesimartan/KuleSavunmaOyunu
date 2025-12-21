namespace KuleSavunmaOyunu.Entities
{
    public class Yol
    {
        private readonly List<Point> noktalar;

        // Dışarıya salt okunur liste sunulur
        public IReadOnlyList<Point> Noktalar => noktalar;

        // Parametresiz ctor: varsayılan rota oluşturur
        public Yol() : this(OlusturVarsayilanNoktalar())
        {
        }

        public Yol(List<Point> noktalar)
        {
            this.noktalar = noktalar ?? new List<Point>();
            HesaplaToplamUzunluk();
        }

        // Fabrika: ön tanımlı rota döner
        public static Yol OlusturVarsayilanYol()
        {
            return new Yol(OlusturVarsayilanNoktalar());
        }

        private static List<Point> OlusturVarsayilanNoktalar()
        {
            return new List<Point>
            {
                new Point(-40, 260),
                new Point(120, 260),
                new Point(120, 80),
                new Point(480, 80),
                new Point(480, 360),
                new Point(720, 360),
                new Point(720, 160),
                new Point(980, 160)
            };
        }

        private double toplamUzunluk;
        public double ToplamUzunluk => toplamUzunluk;

        // Toplam yolu parça parça hesaplar.
        private void HesaplaToplamUzunluk()
        {
            toplamUzunluk = 0.0;
            if (noktalar == null || noktalar.Count < 2)
                return;

            for (int i = 0; i < noktalar.Count - 1; i++)
            {
                var a = noktalar[i];
                var b = noktalar[i + 1];
                double dx = b.X - a.X;
                double dy = b.Y - a.Y;
                toplamUzunluk += Math.Sqrt(dx * dx + dy * dy);
            }
        }

        // Yolun ilk noktası (başlangıç)
        public Point IlkNokta()
        {
            return noktalar.Count > 0 ? noktalar[0] : Point.Empty;
        }

        // Yolun son noktası (çıkış)
        public Point SonNokta()
        {
            return noktalar.Count > 0 ? noktalar[noktalar.Count - 1] : Point.Empty;
        }

        // Verilen bir noktaya en yakın projeksiyonun yol üzerinde kaç piksel ileride olduğunu verir.
        // Eğer rota yoksa 0 döner.
        public double GetClosestDistanceAlongPath(PointF p)
        {
            if (noktalar == null || noktalar.Count < 2)
                return 0.0;

            double px = p.X;
            double py = p.Y;

            double bestDistSq = double.MaxValue;
            double distanceAlong = 0.0;
            double cumulative = 0.0;

            for (int i = 0; i < noktalar.Count - 1; i++)
            {
                var a = noktalar[i];
                var b = noktalar[i + 1];

                double ax = a.X;
                double ay = a.Y;
                double bx = b.X;
                double by = b.Y;

                double dx = bx - ax;
                double dy = by - ay;
                double segLenSq = dx * dx + dy * dy;

                double t;
                if (segLenSq == 0)
                {
                    t = 0;
                }
                else
                {
                    t = ((px - ax) * dx + (py - ay) * dy) / segLenSq;
                    if (t < 0) t = 0;
                    else if (t > 1) t = 1;
                }

                double projx = ax + t * dx;
                double projy = ay + t * dy;

                double rx = px - projx;
                double ry = py - projy;
                double distSq = rx * rx + ry * ry;

                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    double segLen = Math.Sqrt(segLenSq);
                    distanceAlong = cumulative + t * segLen;
                }

                cumulative += Math.Sqrt(segLenSq);
            }

            return distanceAlong;
        }
    }
}