using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Core
{
    public class OkKulesi : Kule
    {
        public OkKulesi(Point konum) : base(konum)
        {
            hasar = 15;
            menzil = 150;
            hiz = 1.0;   // saniyede 1 atış
            fiyat = 100;
        }

        public override void Saldir(List<Dusman> dusmanlar)
        {
            if (!SaldiriHazirMi())
                return;

            // MenziIcindeki düşmanlardan en yakın olanı bul
            var hedef = dusmanlar
                .Where(d => d.Can > 0 && MenziIcindemi(d))
                .OrderBy(d => d.Mesafe(Konum))
                .FirstOrDefault();

            if (hedef != null)
            {
                hedef.HasarAl(hasar);
            }
        }
    }
}
