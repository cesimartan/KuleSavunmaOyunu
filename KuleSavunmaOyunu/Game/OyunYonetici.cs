using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KuleSavunmaOyunu.Core;
using KuleSavunmaOyunu.Entities;
using KuleSavunmaOyunu.Enums;

namespace KuleSavunmaOyunu.Game
{
    public class OyunYonetici
    {
        public List<Kule> Kuleler { get; } = new List<Kule>();
        public List<Dusman> Dusmanlar { get; } = new List<Dusman>();
        public Yol Yol { get; }
        public DalgaYonetici DalgaYonetici { get; }

        // Oyuncu durum bilgileri
        public int Altin { get; private set; }
        public int Can { get; private set; }
        public int Skor { get; private set; }

        public int Dalga => DalgaYonetici.MevcutDalga;

        // Ayarlar (istersen ileride ayrı bir sınıfa taşıyabilirsin)
        private const int BaslangicAltin = 300;
        private const int BaslangicCan = 10;
        private const int DusmanOldurAltin = 20;
        private const int DusmanOldurSkor = 10;
        private const int DusmanGectiCanAzalis = 1;

        public OyunYonetici(Yol yol)
        {
            Yol = yol;
            DalgaYonetici = new DalgaYonetici();
            Altin = BaslangicAltin;
            Can = BaslangicCan;
            Skor = 0;
        }

        /// <summary>
        /// Timer her Tick olduğunda Form tarafından çağrılacak ana metod.
        /// </summary>
        public void Guncelle()
        {
            // 1) Düşmanları ilerlet
            DusmanlariGuncelle();

            // 2) Kulelerin saldırmasını sağla
            KuleleriSaldirt();

            // 3) Ölen düşmanları temizle, altın & skor ver
            OldurenleriIsle();

            // 4) Hedefe ulaşan düşmanları kontrol et, can azalt
            GecenleriIsle();
        }

        /// <summary>
        /// Yeni bir dalga başlatır (listeye yeni düşmanlar ekler).
        /// </summary>
        public void YeniDalgaBaslat()
        {
            var yeniDusmanlar = DalgaYonetici.YeniDalgaOlustur(Yol);
            Dusmanlar.AddRange(yeniDusmanlar);
        }

        private void DusmanlariGuncelle()
        {
            foreach (var d in Dusmanlar)
            {
                d.Guncelle();
            }
        }

        private void KuleleriSaldirt()
        {
            if (Dusmanlar.Count == 0)
                return;

            foreach (var k in Kuleler)
            {
                k.Saldir(Dusmanlar);
            }
        }

        private void OldurenleriIsle()
        {
            // Ölen düşmanları bul
            var olenler = Dusmanlar
                .Where(d => d.Can <= 0)
                .ToList();

            if (!olenler.Any())
                return;

            foreach (var d in olenler)
            {
                Altin += DusmanOldurAltin;
                Skor += DusmanOldurSkor;
                Dusmanlar.Remove(d);
            }
        }

        private void GecenleriIsle()
        {
            var gecenler = Dusmanlar
                .Where(d => d.HedefeUlasti && d.Can > 0)
                .ToList();

            if (!gecenler.Any())
                return;

            foreach (var d in gecenler)
            {
                Can -= DusmanGectiCanAzalis;
                Dusmanlar.Remove(d);
            }

            if (Can < 0)
                Can = 0;
        }

        /// <summary>
        /// Form'da tıklanan noktaya, seçilen tipte kule koymaya çalışır.
        /// Başarılıysa true döner.
        /// </summary>
        public bool KuleKoy(Point konum, KuleTipi tip)
        {
            // 1) Fiyat kontrolü
            int fiyat = KuleFiyati(tip);
            if (Altin < fiyat)
                return false;

            // 2) Yolun üstüne kule koymayı engellemek istersen burada kontrol edebilirsin.
            // Şimdilik basit tutuyoruz: sadece aynı yere iki kule koyma.
            int minMesafe = 40; // kuleler arası minimum mesafe (px)

            bool cokYakin = Kuleler
                .Any(k => Mesafe(k.Konum, konum) < minMesafe);

            if (cokYakin)
                return false;

            // 3) Kuleyi oluştur
            Kule yeniKule = KuleOlustur(tip, konum);
            if (yeniKule == null)
                return false;

            Kuleler.Add(yeniKule);
            Altin -= fiyat;

            return true;
        }

        private int KuleFiyati(KuleTipi tip)
        {
            // Kule sınıflarının içindeki fiyat değerleriyle uyumlu olmasına dikkat et
            switch (tip)
            {
                case KuleTipi.Ok:
                    return 100;
                case KuleTipi.Top:
                    return 250;
                case KuleTipi.Buyu:
                    return 200;
                default:
                    return 9999;
            }
        }

        private Kule KuleOlustur(KuleTipi tip, Point konum)
        {
            switch (tip)
            {
                case KuleTipi.Ok:
                    return new OkKulesi(konum);
                case KuleTipi.Top:
                    return new TopKulesi(konum);
                case KuleTipi.Buyu:
                    return new BuyuKulesi(konum);
                default:
                    return null;
            }
        }

        private double Mesafe(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        public bool OyunBittiMi()
        {
            return Can <= 0;
        }
    }
}
