using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Core
{
    public class TopKulesi : Kule
    {
        // Kurucu: top kulesi temel değerleri atanır.
        public TopKulesi(Point konum) : base(konum)
        {
            hasar = 30;
            menzil = 70;
            hiz = 0.33;   // yaklaşık 3 saniyede 1 atış
            fiyat = 250;
        }

        // Görünür düşmanlara alan hasarı uygular (hız bazlı cooldown kontrolü yapılır).
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