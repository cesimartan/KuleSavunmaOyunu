using KuleSavunmaOyunu.Core;
using KuleSavunmaOyunu.Entities;
using KuleSavunmaOyunu.Game;
using System.Drawing.Drawing2D;

namespace KuleSavunmaOyunu.Rendering
{
    public class CizimMotoru
    {
        // Ana çizim çağrısı: yol, kuleler, düşmanlar çizilir.
        public void Ciz(Graphics g, OyunYonetici oyun)
        {
            if (oyun == null)
                return;

            CizYol(g, oyun.Yol);
            CizKuleler(g, oyun.Kuleler);
            CizDusmanlar(g, oyun.Dusmanlar);
        }

        // Yol polilini kalın bir stroke ile çizer, başlangıç/bitis işaretleri ekler.
        private void CizYol(Graphics g, Yol yol)
        {
            var noktalar = yol.Noktalar;
            if (noktalar == null || noktalar.Count < 2)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var pts = new List<Point>(noktalar).ToArray();

            // Dış hat (kalın)
            using (var disKalem = new Pen(Color.FromArgb(120, 60, 20), 26))
            {
                disKalem.StartCap = LineCap.Round;
                disKalem.EndCap = LineCap.Round;
                disKalem.LineJoin = LineJoin.Round;
                g.DrawLines(disKalem, pts);
            }

            // İç yol rengi
            using (var icKalem = new Pen(Color.SaddleBrown, 18))
            {
                icKalem.StartCap = LineCap.Round;
                icKalem.EndCap = LineCap.Round;
                icKalem.LineJoin = LineJoin.Round;
                g.DrawLines(icKalem, pts);
            }

            // Başlangıç ve bitiş noktalarını işaretle
            Point baslangic = noktalar[0];
            Point bitis = noktalar[noktalar.Count - 1];

            CizIsaretNoktasi(g, baslangic, Color.LimeGreen);
            CizIsaretNoktasi(g, bitis, Color.Red);
        }

        // Yol üzerindeki başlangıç/bitis noktalarını çizer.
        private void CizIsaretNoktasi(Graphics g, Point p, Color renk)
        {
            int r = 15;
            var rect = new Rectangle(p.X - r, p.Y - r, r * 2, r * 2);

            using (var firca = new SolidBrush(renk))
            using (var kalem = new Pen(Color.White, 2))
            {
                g.FillEllipse(firca, rect);
                g.DrawEllipse(kalem, rect);
            }
        }

        // Kulelerin gövdesini, menzilini ve simgesini çizer.
        private void CizKuleler(Graphics g, List<Kule> kuleler)
        {
            int kuleBoyut = 30;

            foreach (var kule in kuleler)
            {
                var (govdeRenk, menzilRenk) = KuleStilGetir(kule);

                // Menzil çemberi
                using (var menzilKalem = new Pen(Color.FromArgb(70, menzilRenk), 2))
                {
                    int r = kule.Menzil;
                    g.DrawEllipse(menzilKalem,
                        kule.Konum.X - r,
                        kule.Konum.Y - r,
                        r * 2,
                        r * 2);
                }

                // Gövde dikdörtgeni
                Rectangle rect = new Rectangle(
                    kule.Konum.X - kuleBoyut / 2,
                    kule.Konum.Y - kuleBoyut / 2,
                    kuleBoyut,
                    kuleBoyut);

                using (var firca = new SolidBrush(govdeRenk))
                using (var kalem = new Pen(Color.White, 2))
                {
                    g.FillRectangle(firca, rect);
                    g.DrawRectangle(kalem, rect);
                }

                // Tip sembolü
                CizKuleSembolu(g, kule, rect);
            }
        }

        // Kule tipi bazlı renk döner.
        private (Color govde, Color menzil) KuleStilGetir(Kule kule)
        {
            if (kule is OkKulesi) return (Color.SteelBlue, Color.LightSkyBlue);
            if (kule is TopKulesi) return (Color.OrangeRed, Color.Gold);
            if (kule is BuyuKulesi) return (Color.MediumPurple, Color.Plum);

            return (Color.DarkSlateGray, Color.LightBlue);
        }

        // Kule iç simgesini (ok/namlu/elmas) çizer.
        private void CizKuleSembolu(Graphics g, Kule kule, Rectangle rect)
        {
            using var ikonFirca = new SolidBrush(Color.FromArgb(200, Color.White));
            using var ikonKalem = new Pen(Color.FromArgb(220, Color.White), 2);

            var inner = Rectangle.Inflate(rect, -6, -6);

            if (kule is OkKulesi)
            {
                // Üçgen şekli
                Point[] tri =
                {
                    new Point(inner.Left + inner.Width / 2, inner.Top),
                    new Point(inner.Right, inner.Bottom),
                    new Point(inner.Left, inner.Bottom)
                };
                g.FillPolygon(ikonFirca, tri);
            }
            else if (kule is TopKulesi)
            {
                // Daire ve küçük namlu çizgisi
                g.FillEllipse(ikonFirca, inner);
                int cx = inner.Left + inner.Width / 2;
                g.DrawLine(ikonKalem,
                    cx, inner.Top + 6,
                    cx, inner.Top - 6);
            }
            else if (kule is BuyuKulesi)
            {
                // Elmas şekli
                Point[] diamond =
                {
                    new Point(inner.Left + inner.Width / 2, inner.Top),
                    new Point(inner.Right, inner.Top + inner.Height / 2),
                    new Point(inner.Left + inner.Width / 2, inner.Bottom),
                    new Point(inner.Left, inner.Top + inner.Height / 2)
                };
                g.FillPolygon(ikonFirca, diamond);
            }
        }

        // Düşmanları çizer; can barı gösterilir.
        private void CizDusmanlar(Graphics g, List<Dusman> dusmanlar)
        {
            int yaricap = 10;

            foreach (var d in dusmanlar)
            {
                if (d.Can <= 0)
                    continue;

                RectangleF daire = new RectangleF(
                    d.Konum.X - yaricap,
                    d.Konum.Y - yaricap,
                    yaricap * 2,
                    yaricap * 2);

                using (var firca = new SolidBrush(Color.DarkRed))
                using (var kalem = new Pen(Color.Black, 1))
                {
                    g.FillEllipse(firca, daire);
                    g.DrawEllipse(kalem, daire);
                }

                // Can barı hesapla ve çiz
                float canOrani = d.MaksCan > 0 ? d.Can / (float)d.MaksCan : 0f;
                if (canOrani < 0) canOrani = 0;
                if (canOrani > 1) canOrani = 1;

                RectangleF arkaBar = new RectangleF(
                    d.Konum.X - yaricap,
                    d.Konum.Y - yaricap - 6,
                    yaricap * 2,
                    4);

                RectangleF onBar = new RectangleF(
                    arkaBar.X,
                    arkaBar.Y,
                    arkaBar.Width * canOrani,
                    arkaBar.Height);

                using (var arkaFirca = new SolidBrush(Color.DarkGray))
                using (var onFirca = new SolidBrush(Color.LimeGreen))
                {
                    g.FillRectangle(arkaFirca, arkaBar);
                    g.FillRectangle(onFirca, onBar);
                }
            }
        }
    }
}