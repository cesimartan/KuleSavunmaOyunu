namespace KuleSavunmaOyunu.Entities
{
    // Yol boyunca ilerleyen düşman sınıfı
    public class Dusman
    {
        // Can bilgisi (dışarıdan sadece okunur)
        public int Can { get; private set; }

        // Ekrandaki konum
        public PointF Konum { get; private set; }

        // Hareket hızı (piksel / tick benzeri)
        public float Hiz { get; private set; }

        // Başlangıç spawn gecikmesi (tick cinsinden)
        private int baslangicGecikmeTicks;
        public bool Aktif => baslangicGecikmeTicks <= 0;
        public int MaksCan { get; private set; }

        // Takip edilen yolun noktaları ve hedef index'i
        private readonly List<Point> yolNoktalari;
        private int hedefIndex;
        private bool hedefeUlasti;

        public bool HedefeUlasti => hedefeUlasti;

        // Kurucu: yol referansı, can, hız ve spawn gecikmesi alır
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

        // Her tick çağrılır: düşmanı bir sonraki hedefe doğru hareket ettirir
        public void Guncelle()
        {
            if (hedefeUlasti || Can <= 0)
                return;

            // Spawn gecikmesi devam ediyorsa bekle
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

            // Küçük mesafelerde doğrudan hedefe sıçra ve index'i ilerlet
            if (mesafe <= Hiz)
            {
                Konum = new PointF(hedefNokta.X, hedefNokta.Y);
                hedefIndex++;

                if (hedefIndex >= yolNoktalari.Count - 1)
                    hedefeUlasti = true;

                return;
            }

            // Normale edilmiş yön ile ilerle
            float nx = dx / mesafe;
            float ny = dy / mesafe;

            Konum = new PointF(Konum.X + nx * Hiz, Konum.Y + ny * Hiz);
        }

        // Kulelerin uyguladığı hasarı işler
        public void HasarAl(int miktar)
        {
            if (miktar <= 0 || Can <= 0)
                return;

            Can -= miktar;
            if (Can < 0)
                Can = 0;
        }

        // Bir noktaya olan uzaklığı döner (kule menzil kontrolü için)
        public float Mesafe(Point p)
        {
            float dx = Konum.X - p.X;
            float dy = Konum.Y - p.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
