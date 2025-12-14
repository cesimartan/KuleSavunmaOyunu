using System.Collections.Generic;
using System.Drawing;
using KuleSavunmaOyunu.Core;
using KuleSavunmaOyunu.Entities;
using KuleSavunmaOyunu.Game;
using System.Drawing.Drawing2D;


namespace KuleSavunmaOyunu.Rendering
{
    public class CizimMotoru
    {
        // Ana çizim fonksiyonu
        public void Ciz(Graphics g, OyunYonetici oyun)
        {
            if (oyun == null)
                return;

            // Arka planı temizlemiyoruz, panelin BackColor'ı zaten var.

            CizYol(g, oyun.Yol);
            CizKuleler(g, oyun.Kuleler);
            CizDusmanlar(g, oyun.Dusmanlar);
        }

        private void CizYol(Graphics g, Yol yol)
        {
            var noktalar = yol.Noktalar;
            if (noktalar == null || noktalar.Count < 2)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var pts = new List<Point>(noktalar).ToArray();

            // Dış hat
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
        }


        private void CizKuleler(Graphics g, List<Kule> kuleler)
        {
            int kuleBoyut = 30;

            foreach (var kule in kuleler)
            {
                // Menzil çemberi
                using (var menzilKalem = new Pen(Color.FromArgb(80, Color.LightBlue), 1))
                {
                    int r = kule.Menzil;
                    g.DrawEllipse(menzilKalem,
                        kule.Konum.X - r,
                        kule.Konum.Y - r,
                        r * 2,
                        r * 2);
                }

                // Kule gövdesi (kare)
                Rectangle rect = new Rectangle(
                    kule.Konum.X - kuleBoyut / 2,
                    kule.Konum.Y - kuleBoyut / 2,
                    kuleBoyut,
                    kuleBoyut);

                using (var firca = new SolidBrush(Color.DarkSlateGray))
                using (var kalem = new Pen(Color.White, 2))
                {
                    g.FillRectangle(firca, rect);
                    g.DrawRectangle(kalem, rect);
                }
            }
        }

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

                // Can barı
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
