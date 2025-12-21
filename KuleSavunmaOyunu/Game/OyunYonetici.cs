using KuleSavunmaOyunu.Core;
using KuleSavunmaOyunu.Entities;
using KuleSavunmaOyunu.Enums;

namespace KuleSavunmaOyunu.Game
{
    public class OyunYonetici
    {
        // Oyundaki kule ve düşman listeleri
        public List<Kule> Kuleler { get; } = new List<Kule>();
        public List<Dusman> Dusmanlar { get; } = new List<Dusman>();
        public Yol Yol { get; }
        public DalgaYonetici DalgaYonetici { get; }

        // Spawn kuyrugu ve zamanlayıcı
        private readonly Queue<Dusman> bekleyenDusmanlar = new Queue<Dusman>();
        private DateTime sonSpawnZamani = DateTime.MinValue;

        // Zaman ayarları (saniye cinsinden)
        private const double DusmanSpawnAraligiSn = 0.6;   // düşmanlar arası aralık
        private const double DalgaArasiBeklemeSn = 1.0;    // dalgalar arası bekleme

        private DateTime? dalgaBittiZamani = null;

        // Oyuncu durumu
        public int Altin { get; private set; }
        public int Can { get; private set; }
        public int Skor { get; private set; }

        public int Dalga => DalgaYonetici.MevcutDalga;

        // Sabit ayarlar
        private const int BaslangicAltin = 300;
        private const int BaslangicCan = 10;
        private const int DusmanOldurAltin = 20;
        private const int DusmanOldurSkor = 10;
        private const int DusmanGectiCanAzalis = 1;

        // Maksimum kule sayısı
        private const int MaksimumKule = 3;
        public int MaksimumKuleSayisi => MaksimumKule;

        // Kule merkezleri arası minimum mesafe (px)
        private const int MinKuleMerkezMesafesi = 150;
        public int MinimumKuleMerkezMesafesi => MinKuleMerkezMesafesi;

        // Kurucu: yol referansı ve başlangıç değerleri atanır.
        public OyunYonetici(Yol yol)
        {
            Yol = yol;
            DalgaYonetici = new DalgaYonetici();
            Altin = BaslangicAltin;
            Can = BaslangicCan;
            Skor = 0;
        }

        // Kuyruktan düşmanları belirli aralıklarla sahaya ekler.
        private void DusmanSpawnEt()
        {
            if (bekleyenDusmanlar.Count == 0)
                return;

            // Spawn aralığı dolmadıysa çık
            if ((DateTime.Now - sonSpawnZamani).TotalSeconds < DusmanSpawnAraligiSn)
                return;

            Dusmanlar.Add(bekleyenDusmanlar.Dequeue());
            sonSpawnZamani = DateTime.Now;
        }

        // Dalga yönetimi: dalga bittiğinde otomatik yeni dalga başlatma kontrolü.
        private void OtomatikDalgaKontrol()
        {
            if (Can <= 0) // oyun bitti ise yeni dalga yok
                return;

            // Hâlâ aktif düşman veya spawn kuyruğu varsa dalga devam ediyor
            if (Dusmanlar.Count > 0 || bekleyenDusmanlar.Count > 0)
            {
                dalgaBittiZamani = null;
                return;
            }

            // Yeni sona erdi: zamanı işaretle
            if (dalgaBittiZamani == null)
            {
                dalgaBittiZamani = DateTime.Now;
                return;
            }

            // Bekleme süresi dolduysa yeni dalga başlat
            if ((DateTime.Now - dalgaBittiZamani.Value).TotalSeconds >= DalgaArasiBeklemeSn)
            {
                YeniDalgaBaslat();
                dalgaBittiZamani = null;
            }
        }

        // Ana güncelleme: timer tick tarafından çağrılır.
        public void Guncelle()
        {
            // 0) Bekleyen düşmanları spawn et
            DusmanSpawnEt();

            // 1) Düşmanları hareket ettir/güncelle
            DusmanlariGuncelle();

            // 2) Kulelerin saldırmasını sağla
            KuleleriSaldirt();

            // 3) Ölen düşmanları işleyip ödül ver
            OldurenleriIsle();

            // 4) Hedefe ulaşan düşmanları işle (can azalt)
            GecenleriIsle();

            // 5) Dalga kontrolü
            OtomatikDalgaKontrol();

            // Eğer sahada hiç düşman yoksa yeni dalga başlat
            if (Can > 0 && Dusmanlar.Count == 0)
                YeniDalgaBaslat();
        }

        // DalgaYonetici'den yeni düşmanlar alıp spawn kuyruğuna ekler.
        public void YeniDalgaBaslat()
        {
            var yeniDusmanlar = DalgaYonetici.YeniDalgaOlustur(Yol);
            foreach (var d in yeniDusmanlar)
                bekleyenDusmanlar.Enqueue(d);

            dalgaBittiZamani = null;
        }

        // Her düşmanın Guncelle metodunu çağırır.
        private void DusmanlariGuncelle()
        {
            foreach (var d in Dusmanlar)
            {
                d.Guncelle();
            }
        }

        // Her kule için hedef listesini ileterek saldırı yaptırır.
        private void KuleleriSaldirt()
        {
            var hedefler = Dusmanlar.Where(d => d.Can > 0 && d.Aktif).ToList();
            if (hedefler.Count == 0)
                return;

            foreach (var k in Kuleler)
                k.Saldir(hedefler);
        }

        // Ölen düşmanları temizler ve ödül verir.
        private void OldurenleriIsle()
        {
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

        // Hedefe ulaşan düşmanları işleyip canı azaltır.
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

        // Form üzerinde tıklanan noktaya seçili tipte kule koymaya çalışır.
        // Başarılıysa true döner.
        public bool KuleKoy(Point konum, KuleTipi tip)
        {
            // Maksimum kule kontrolü
            if (Kuleler.Count >= MaksimumKule)
                return false;

            // 1) Fiyat kontrolü
            int fiyat = KuleFiyati(tip);
            if (Altin < fiyat)
                return false;

            // 2) Yolun üzerinde engelle
            if (Yol != null && YolUzerindeMi(konum))
                return false;

            // 3) Kuleler arası minimum mesafe kontrolü
            int minMesafe = MinimumKuleMerkezMesafesi;
            bool cokYakin = Kuleler
                .Any(k => Mesafe(k.Konum, konum) < minMesafe);

            if (cokYakin)
                return false;

            // 4) Kule oluştur ve listeye ekle
            Kule yeniKule = KuleOlustur(tip, konum);
            if (yeniKule == null)
                return false;

            // Kuleye oyundaki yolu ata (kule de yol bilgisine ihtiyaç duyabilir)
            yeniKule.Yol = this.Yol;

            Kuleler.Add(yeniKule);
            Altin -= fiyat;

            return true;
        }

        // Harici kodlar için güvenli fiyat sorgulama
        public int KuleFiyatiGetir(KuleTipi tip)
        {
            return KuleFiyati(tip);
        }

        // Kule tipine göre fiyat döner.
        private int KuleFiyati(KuleTipi tip)
        {
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

        // Tipe göre ilgili kule sınıfını örnekler.
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

        // İki nokta arasındaki öklid mesafesini döner.
        private double Mesafe(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        // Bir noktanın yol polilin üzerindeki herhangi bir segmentine yakın olup olmadığını kontrol eder.
        private bool YolUzerindeMi(Point konum)
        {
            var noktalar = Yol?.Noktalar;
            if (noktalar == null || noktalar.Count < 2)
                return false;

            // CizimMotoru dış hat kalınlığına göre yarıçap
            const double yolYaricap = 13.0;

            for (int i = 0; i < noktalar.Count - 1; i++)
            {
                if (DistancePointToSegment(konum, noktalar[i], noktalar[i + 1]) <= yolYaricap)
                    return true;
            }

            return false;
        }

        // Noktadan doğru parçasına en yakın uzaklığı hesaplar.
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
                // a ve b aynı nokta ise doğrudan uzaklık
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

        // Oyun bitiş kontrolü
        public bool OyunBittiMi()
        {
            return Can <= 0;
        }
    }
}