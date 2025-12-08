using System.Collections.Generic;
using System.Drawing;
using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Game
{
    public class DalgaYonetici
    {
        // 1'den başlayan dalga numarası
        public int MevcutDalga { get; private set; } = 0;

        // Temel ayarlar
        private readonly int temelDusmanSayisi = 4;
        private readonly int dusmanSayisiArtis = 2;
        private readonly int temelCan = 40;
        private readonly int canArtis = 10;
        private readonly float temelHiz = 1.2f;
        private readonly float hizArtis = 0.1f;

        public DalgaYonetici()
        {
        }

        /// <summary>
        /// Yeni bir düşman dalgası üretir ve dalga sayacını 1 arttırır.
        /// </summary>
        public List<Dusman> YeniDalgaOlustur(Yol yol)
        {
            MevcutDalga++;

            int dusmanSayisi = temelDusmanSayisi + (MevcutDalga - 1) * dusmanSayisiArtis;
            int dusmanCan = temelCan + (MevcutDalga - 1) * canArtis;
            float dusmanHiz = temelHiz + (MevcutDalga - 1) * hizArtis;

            var liste = new List<Dusman>();

            for (int i = 0; i < dusmanSayisi; i++)
            {
                // Aynı yolu kullanan düşmanlar
                var yolKopya = new List<Point>(yol.Noktalar);
                var dusman = new Dusman(yolKopya, dusmanCan, dusmanHiz);
                liste.Add(dusman);
            }

            return liste;
        }
    }
}
