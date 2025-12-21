using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Core
{
    // Kulelerin ortak davranış ve durumlarını tutan soyut sınıf.
    public abstract class Kule : ISaldirabilir, IYukseltilebilir
    {
        // Temel özellikler (encapsulation için protected alanlar)
        protected int hasar;                  // bir atışın verdiği hasar
        protected int menzil;                 // piksel cinsinden etki yarıçapı
        protected double hiz;                 // saniyedeki atış sayısı (örn. 1.0 => 1 atış/sn)
        protected int fiyat;                  // oluşturma maliyeti

        private Point konum;                  // kule merkezi
        protected DateTime sonSaldiriZamani;  // son yapılan saldırı zamanı (cooldown kontrolü)

        // Oyun yöneticisinin atayacağı yol referansı (kule yol bilgisi kullanabilir)
        public Yol Yol { get; set; }

        // Konum güvenli şekilde okunur; dışarıdan set edilemez.
        public Point Konum
        {
            get { return konum; }
            private set { konum = value; }
        }

        // Salt okunur özellik erişimleri
        public int Hasar => hasar;
        public int Menzil => menzil;
        public double Hiz => hiz;
        public int Fiyat => fiyat;

        // Oluşturucu: konumu ayarlar ve saldırı zamanlayıcısını sıfırlar.
        protected Kule(Point konum)
        {
            Konum = konum;
            sonSaldiriZamani = DateTime.MinValue;
        }

        // Alt sınıflar tarafından hedeflere saldırı mantığı burada uygulanacak.
        public abstract void Saldir(List<Dusman> dusmanlar);

        // Varsayılan yükseltme davranışı: hasar ve menzili artırır. Gerekirse override edilebilir.
        public virtual void Yukselt()
        {
            hasar += 5;
            menzil += 10;
        }

        // Saldırı hızı (hiz) bazlı cooldown kontrolü.
        // Gerekli süre geçtiyse sonSaldiriZamani güncellenir ve true döner.
        protected bool SaldiriHazirMi()
        {
            if (hiz <= 0)
                return true;

            double gerekenSure = 1.0 / hiz; // ör: hiz = 1 => 1s, hiz = 2 => 0.5s
            var gecenSure = (DateTime.Now - sonSaldiriZamani).TotalSeconds;

            if (gecenSure >= gerekenSure)
            {
                sonSaldiriZamani = DateTime.Now;
                return true;
            }

            return false;
        }

        // Bir düşmanın kule menzili içinde olup olmadığını kontrol eder.
        protected bool MenziIcindemi(Dusman dusman)
        {
            return dusman.Mesafe(Konum) <= menzil;
        }
    }
}