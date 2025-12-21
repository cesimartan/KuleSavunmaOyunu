using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Game
{
    public class DalgaYonetici
    {
        // 1'den başlayan dalga numarası
        public int MevcutDalga { get; private set; } = 0;

        // Temel ayarlar
        private readonly int temelDusmanSayisi = 8;
        private readonly int dusmanSayisiArtis = 4;
        private readonly int temelCan = 80;
        private readonly int canArtis = 20;
        private readonly float temelHiz = 1.2f;
        private readonly float hizArtis = 0.2f;

        public DalgaYonetici()
        {
        }


        // Yeni bir düşman dalgası üretir ve dalga sayacını 1 arttırır.

        public List<Dusman> YeniDalgaOlustur(Yol yol)
        {
            MevcutDalga++;

            int dusmanSayisi = temelDusmanSayisi + (MevcutDalga - 1) * dusmanSayisiArtis;
            int dusmanCan = (int)(temelCan * Math.Pow(1.20, MevcutDalga - 1));
            float dusmanHiz = (float)(temelHiz * Math.Pow(1.08, MevcutDalga - 1));
            var liste = new List<Dusman>();

            int spawnAraligiTicks = 25; // timer 30ms ise ~0.3sn

            for (int i = 0; i < dusmanSayisi; i++)
            {
                var yolKopya = new List<Point>(yol.Noktalar);
                int gecikme = i * spawnAraligiTicks;

                var dusman = new Dusman(yolKopya, dusmanCan, dusmanHiz, gecikme);
                liste.Add(dusman);
            }

            return liste;
        }
    }
}
