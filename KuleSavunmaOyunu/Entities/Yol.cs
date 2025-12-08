using System.Collections.Generic;
using System.Drawing;

namespace KuleSavunmaOyunu.Entities
{
    public class Yol
    {
        private readonly List<Point> noktalar;

        public IReadOnlyList<Point> Noktalar => noktalar;

        public Yol(List<Point> noktalar)
        {
            this.noktalar = noktalar ?? new List<Point>();
        }

        public Point IlkNokta()
        {
            return noktalar.Count > 0 ? noktalar[0] : Point.Empty;
        }

        public Point SonNokta()
        {
            return noktalar.Count > 0 ? noktalar[noktalar.Count - 1] : Point.Empty;
        }
    }
}
