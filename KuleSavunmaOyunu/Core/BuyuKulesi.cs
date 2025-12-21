using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Core
{
    public class BuyuKulesi : Kule
    {
        public BuyuKulesi(Point konum) : base(konum)
        {
            hasar = 25;
            menzil = 80;
            hiz = 0.66;   // yaklaşık 1.5 sn'de 1 atış (1/0.66 ≈ 1.5)
            fiyat = 200;
        }

        public override void Saldir(List<Dusman> dusmanlar)
        {
            if (!SaldiriHazirMi())
                return;

            var hedefler = dusmanlar
                .Where(d => d.Can > 0 && d.Aktif && MenziIcindemi(d))
                .OrderBy(d => d.Mesafe(Konum))
                .Take(5)
                .ToList();

            if (!hedefler.Any())
                return;

            foreach (var dusman in hedefler)
            {
                dusman.HasarAl(hasar);
            }
        }
    }
}