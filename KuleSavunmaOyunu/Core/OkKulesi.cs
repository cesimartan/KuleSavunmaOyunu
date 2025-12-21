using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Core
{
    public class OkKulesi : Kule
    {
        private Dusman kilitliHedef;

        public OkKulesi(Point konum) : base(konum)
        {
            hasar = 15;
            menzil = 100;
            hiz = 1.0;   // saniyede 1 atış
            fiyat = 100;
        }

        public override void Saldir(List<Dusman> dusmanlar)
        {
            // Mevcut kilitli hedef yoksa ya da hedef ölmüş/aktif değil/menzil dışına çıktıysa en yakın aktif hedefe kilitlen
            if (kilitliHedef == null || kilitliHedef.Can <= 0 || !kilitliHedef.Aktif || !MenziIcindemi(kilitliHedef))
            {
                kilitliHedef = dusmanlar
                    .Where(d => d.Can > 0 && d.Aktif && MenziIcindemi(d))
                    .OrderBy(d => d.Mesafe(Konum))
                    .FirstOrDefault();
            }

            if (kilitliHedef == null)
                return;

            // Saldırı hazırsa kilitli hedefe tek atış yap
            if (!SaldiriHazirMi())
                return;

            kilitliHedef.HasarAl(hasar);

            // Hedef öldüyse kilidi temizle
            if (kilitliHedef.Can <= 0)
                kilitliHedef = null;
        }
    }
}