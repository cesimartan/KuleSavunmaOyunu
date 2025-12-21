using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Core
{
    public class TopKulesi : Kule
    {
        public TopKulesi(Point konum) : base(konum)
        {
            hasar = 30;
            menzil = 70;
            hiz = 0.33;   // yakl. 3 saniyede 1 atış (1/0.33 ≈ 3 sn)
            fiyat = 250;
        }

        public override void Saldir(List<Dusman> dusmanlar)
        {
            if (!SaldiriHazirMi())
                return;

            var hedefler = dusmanlar
                .Where(d => d.Can > 0 && d.Aktif && MenziIcindemi(d))
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