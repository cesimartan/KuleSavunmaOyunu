using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;
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

        // Dalga/spawn kontrolü
        private readonly Queue<Dusman> bekleyenDusmanlar = new Queue<Dusman>();
        private DateTime sonSpawnZamani = DateTime.MinValue;

        private const double DusmanSpawnAraligiSn = 0.6;   // düşmanlar arası mesafe (arttır = daha seyrek)
        private const double DalgaArasiBeklemeSn = 2.0;    // dalga arası bekleme

        private DateTime? dalgaBittiZamani = null;

        // Oyuncu durum bilgileri
        public int Altin { get; private set; }
        public int Can { get; private set; }
        public int Skor { get; private set; }

        public int Dalga => DalgaYonetici.MevcutDalga;

        // Ayarlar
        private const int BaslangicAltin = 300;
        private const int BaslangicCan = 10;
        private const int DusmanOldurAltin = 20;
        private const int DusmanOldurSkor = 10;
        private const int DusmanGectiCanAzalis = 1;

        // Yeni: maksimum kule sayısı
        private const int MaksimumKule = 3;
        public int MaksimumKuleSayisi => MaksimumKule;

        public OyunYonetici(Yol yol)
        {
            Yol = yol;
            DalgaYonetici = new DalgaYonetici();
            Altin = BaslangicAltin;
            Can = BaslangicCan;
            Skor = 0;
        }

        private void DusmanSpawnEt()
        {
            if (bekleyenDusmanlar.Count == 0)
                return;

            // İlk spawn hemen gelsin diye sonSpawnZamani MinValue
            if ((DateTime.Now - sonSpawnZamani).TotalSeconds < DusmanSpawnAraligiSn)
                return;

            Dusmanlar.Add(bekleyenDusmanlar.Dequeue());
            sonSpawnZamani = DateTime.Now;
        }

        private void OtomatikDalgaKontrol()
        {
            if (Can <= 0) // oyun bittiyse yeni dalga başlatma
                return;

            // Dalga hâlâ sürüyor (aktif düşman var veya spawn kuyruğu dolu)
            if (Dusmanlar.Count > 0 || bekleyenDusmanlar.Count > 0)
            {
                dalgaBittiZamani = null;
                return;
            }

            // Dalga yeni bitti: zamanı işaretle
            if (dalgaBittiZamani == null)
            {
                dalgaBittiZamani = DateTime.Now;
                return;
            }

            // Bekleme süresi dolduysa yeni dalga
            if ((DateTime.Now - dalgaBittiZamani.Value).TotalSeconds >= DalgaArasiBeklemeSn)
            {
                YeniDalgaBaslat();
                dalgaBittiZamani = null;
            }
        }

        // Timer her Tick olduğunda Form tarafından çağrılacak ana metod.

        public void Guncelle()
        {
            // 0) Bekleyen düşmanları aralıkla sahaya sal
            DusmanSpawnEt();

            // 1) Düşmanları ilerlet
            DusmanlariGuncelle();

            // 2) Kulelerin saldırmasını sağla
            KuleleriSaldirt();

            // 3) Ölen düşmanları temizle, altın & skor ver
            OldurenleriIsle();

            // 4) Hedefe ulaşan düşmanları kontrol et, can azalt
            GecenleriIsle();

            // 5) Dalga bittiyse otomatik yeni dalga
            OtomatikDalgaKontrol();

            // Dalga bitti mi? (hiç düşman kalmadıysa) yeni dalga başlat
            if (Can > 0 && Dusmanlar.Count == 0)
                YeniDalgaBaslat();
        }


        // Yeni bir dalga başlatır (listeye yeni düşmanlar ekler).

        public void YeniDalgaBaslat()
        {
            var yeniDusmanlar = DalgaYonetici.YeniDalgaOlustur(Yol);
            foreach (var d in yeniDusmanlar)
                bekleyenDusmanlar.Enqueue(d);

            dalgaBittiZamani = null;
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
            var hedefler = Dusmanlar.Where(d => d.Can > 0 && d.Aktif).ToList();
            if (hedefler.Count == 0)
                return;

            foreach (var k in Kuleler)
                k.Saldir(hedefler);
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


        // Form'da tıklanan noktaya, seçilen tipte kule koymaya çalışır.
        // Başarılıysa true döner.

        public bool KuleKoy(Point konum, KuleTipi tip)
        {
            // Yeni: maksimum kule kontrolü
            if (Kuleler.Count >= MaksimumKule)
                return false;

            // 1) Fiyat kontrolü
            int fiyat = KuleFiyati(tip);
            if (Altin < fiyat)
                return false;

            // 2) Yolun üstüne kule koymayı engellemek
            if (Yol != null && YolUzerindeMi(konum))
                return false;

            // 3) Şimdilik basit tutuyoruz: sadece aynı yere iki kule koyma.
            int minMesafe = 40; // kuleler arası minimum mesafe (px)

            bool cokYakin = Kuleler
                .Any(k => Mesafe(k.Konum, konum) < minMesafe);

            if (cokYakin)
                return false;

            // 4) Kuleyi oluştur
            Kule yeniKule = KuleOlustur(tip, konum);
            if (yeniKule == null)
                return false;

            // Önemli: kuleye oyundaki yolu ata, böylece kule yol-segment kontrolü yapabilir
            yeniKule.Yol = this.Yol;

            Kuleler.Add(yeniKule);
            Altin -= fiyat;

            return true;
        }

        // UI gibi katmanlarin fiyati ogrenebilmesi icin guvenli erisim
        public int KuleFiyatiGetir(KuleTipi tip)
        {
            return KuleFiyati(tip);
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

        // Yolun çizildiği kalınlığa uygun olarak, tıklanan konumun yol üzerinde olup olmadığını kontrol eder.
        private bool YolUzerindeMi(Point konum)
        {
            var noktalar = Yol?.Noktalar;
            if (noktalar == null || noktalar.Count < 2)
                return false;

            // CizimMotoru'da dış hat kalınlığı 26, yani yarıçap ~13 px. Bu değeri kullanıyoruz.
            const double yolYaricap = 13.0;

            for (int i = 0; i < noktalar.Count - 1; i++)
            {
                if (DistancePointToSegment(konum, noktalar[i], noktalar[i + 1]) <= yolYaricap)
                    return true;
            }

            return false;
        }

        // Noktadan doğru parçasına olan en kısa mesafeyi döner.
        private double DistancePointToSegment(Point p, Point a, Point b)
        {
            double px = p.X;
            double py = p.Y;
            double ax = a.X;
            double ay = a.Y;
            double bx = b.X;
            double by = b.Y;

            double dx = bx - ax;
            double dy = by - ay;

            if (dx == 0 && dy == 0)
            {
                // a == b
                double ddx = px - ax;
                double ddy = py - ay;
                return Math.Sqrt(ddx * ddx + ddy * ddy);
            }

            double t = ((px - ax) * dx + (py - ay) * dy) / (dx * dx + dy * dy);
            if (t < 0) t = 0;
            else if (t > 1) t = 1;

            double projx = ax + t * dx;
            double projy = ay + t * dy;

            double rx = px - projx;
            double ry = py - projy;

            return Math.Sqrt(rx * rx + ry * ry);
        }

        public bool OyunBittiMi()
        {
            return Can <= 0;
        }
    }
}