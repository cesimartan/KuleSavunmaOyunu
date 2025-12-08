using System;
using System.Collections.Generic;
using System.Drawing;

namespace KuleSavunmaOyunu.Entities
{
    public class Dusman
    {
        // Can bilgisi (Encapsulation)
        public int Can { get; private set; }

        // Oyun alanındaki konumu
        public PointF Konum { get; private set; }

        // Hareket hızı (piksel / tick gibi düşünebilirsin)
        public float Hiz { get; private set; }

        // Yol üzerindeki noktalar
        private readonly List<Point> yolNoktalari;
        private int hedefIndex;
        private bool hedefeUlasti;

        public bool HedefeUlasti => hedefeUlasti;

        public Dusman(List<Point> yol, int baslangicCan = 50, float hiz = 1.5f)
        {
            if (yol == null || yol.Count == 0)
                throw new ArgumentException("Yol boş olamaz", nameof(yol));

            yolNoktalari = yol;
            hedefIndex = 0;
            Konum = yolNoktalari[0];
            Can = baslangicCan;
            Hiz = hiz;
            hedefeUlasti = false;
        }

        /// <summary>
        /// Her oyun tick'inde çağrılacak: düşmanı bir sonraki yol noktasına doğru hareket ettirir.
        /// </summary>
        public void Guncelle()
        {
            if (hedefeUlasti || Can <= 0)
                return;

            if (hedefIndex >= yolNoktalari.Count - 1)
            {
                // Artık son noktadayız
                hedefeUlasti = true;
                return;
            }

            Point hedefNokta = yolNoktalari[hedefIndex + 1];

            // Hedef noktanın yön vektörünü hesapla
            float dx = hedefNokta.X - Konum.X;
            float dy = hedefNokta.Y - Konum.Y;
            float mesafe = (float)Math.Sqrt(dx * dx + dy * dy);

            if (mesafe < 0.1f)
            {
                // Hedef noktaya çok yaklaştıysak bir sonraki noktaya geç
                hedefIndex++;
                if (hedefIndex >= yolNoktalari.Count - 1)
                    hedefeUlasti = true;

                return;
            }

            // Normalize et ve hızla çarp
            float nx = dx / mesafe;
            float ny = dy / mesafe;

            Konum = new PointF(
                Konum.X + nx * Hiz,
                Konum.Y + ny * Hiz
            );
        }

        /// <summary>
        /// Kuleler bu metodu kullanarak hasar verir.
        /// </summary>
        public void HasarAl(int miktar)
        {
            if (miktar <= 0 || Can <= 0)
                return;

            Can -= miktar;
            if (Can < 0)
                Can = 0;
        }

        /// <summary>
        /// Verilen bir noktaya olan uzaklığı döner (kule menzil hesabı için).
        /// </summary>
        public float Mesafe(Point p)
        {
            float dx = Konum.X - p.X;
            float dy = Konum.Y - p.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
