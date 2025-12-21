namespace KuleSavunmaOyunu.Entities
{
    public class Yol
    {
        private readonly List<Point> noktalar;

        public IReadOnlyList<Point> Noktalar => noktalar;

        // Parameterless constructor that creates a sensible default route similar to the provided image.
        public Yol() : this(OlusturVarsayilanNoktalar())
        {
        }

        public Yol(List<Point> noktalar)
        {
            this.noktalar = noktalar ?? new List<Point>();
            HesaplaToplamUzunluk();
        }

        // Factory helper that returns a Yol using a predefined route.
        // Adjust coordinates if you need the path to match your canvas size.
        public static Yol OlusturVarsayilanYol()
        {
            return new Yol(OlusturVarsayilanNoktalar());
        }

        private static List<Point> OlusturVarsayilanNoktalar()
        {
            // Coordinates chosen to approximate the winding route in the reference image.
            // Tweak values to fit your game canvas / form resolution.
            return new List<Point>
            {
                new Point(-40, 260),  // start off-left
                new Point(120, 260),  // go right
                new Point(120, 80),   // up
                new Point(480, 80),   // right (top horizontal)
                new Point(480, 360),  // down (long vertical)
                new Point(720, 360),  // right (bottom horizontal)
                new Point(720, 160),  // up (short vertical)
                new Point(980, 160)   // exit right
            };
        }

        private double toplamUzunluk;
        public double ToplamUzunluk => toplamUzunluk;

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

        public Point IlkNokta()
        {
            return noktalar.Count > 0 ? noktalar[0] : Point.Empty;
        }

        public Point SonNokta()
        {
            return noktalar.Count > 0 ? noktalar[noktalar.Count - 1] : Point.Empty;
        }

        // Returns the distance along the path (from start) of the closest projection
        // of point p onto the polyline. If path empty, returns 0.
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