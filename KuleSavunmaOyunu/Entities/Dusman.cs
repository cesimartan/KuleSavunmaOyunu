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

        //başlangıç gecikmesi (tick)
        private int baslangicGecikmeTicks;
        public bool Aktif => baslangicGecikmeTicks <= 0;
        public int MaksCan { get; private set; }

        // Yol üzerindeki noktalar
        private readonly List<Point> yolNoktalari;
        private int hedefIndex;
        private bool hedefeUlasti;

        public bool HedefeUlasti => hedefeUlasti;

        public Dusman(List<Point> yol, int baslangicCan = 50, float hiz = 1.5f, int baslangicGecikmeTicks = 0)
        {
            if (yol == null || yol.Count == 0)
                throw new ArgumentException("Yol boş olamaz", nameof(yol));

            yolNoktalari = yol;
            hedefIndex = 0;
            Konum = yolNoktalari[0];

            Can = baslangicCan;
            MaksCan = baslangicCan;

            Hiz = hiz;
            hedefeUlasti = false;

            this.baslangicGecikmeTicks = baslangicGecikmeTicks;
        }

        // Her oyun tick'inde çağrılacak: düşmanı bir sonraki yol noktasına doğru hareket ettirir.
        public void Guncelle()
        {
            if (hedefeUlasti || Can <= 0)
                return;

            // Spawn gecikmesi
            if (baslangicGecikmeTicks > 0)
            {
                baslangicGecikmeTicks--;
                return;
            }

            if (hedefIndex >= yolNoktalari.Count - 1)
            {
                hedefeUlasti = true;
                return;
            }

            Point hedefNokta = yolNoktalari[hedefIndex + 1];

            float dx = hedefNokta.X - Konum.X;
            float dy = hedefNokta.Y - Konum.Y;
            float mesafe = (float)Math.Sqrt(dx * dx + dy * dy);

            // Köşe takılma fix
            if (mesafe <= Hiz)
            {
                Konum = new PointF(hedefNokta.X, hedefNokta.Y);
                hedefIndex++;

                if (hedefIndex >= yolNoktalari.Count - 1)
                    hedefeUlasti = true;

                return;
            }

            float nx = dx / mesafe;
            float ny = dy / mesafe;

            Konum = new PointF(Konum.X + nx * Hiz, Konum.Y + ny * Hiz);
        }

        // Kuleler bu metodu kullanarak hasar verir.

        public void HasarAl(int miktar)
        {
            if (miktar <= 0 || Can <= 0)
                return;

            Can -= miktar;
            if (Can < 0)
                Can = 0;
        }

        
       // Verilen bir noktaya olan uzaklığı döner (kule menzil hesabı için).
        
        public float Mesafe(Point p)
        {
            float dx = Konum.X - p.X;
            float dy = Konum.Y - p.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
