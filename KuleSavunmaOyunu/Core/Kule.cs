using KuleSavunmaOyunu.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using KuleSavunmaOyunu.Entities;

namespace KuleSavunmaOyunu.Core
{
    public abstract class Kule : ISaldirabilir, IYukseltilebilir
    {
        // Encapsulation: field + property
        protected int hasar;
        protected int menzil;
        protected double hiz;      // saniyede 1 atış için 1.0
        protected int fiyat;

        private Point konum;
        protected DateTime sonSaldiriZamani;

        // Yol referansı (Oyun yöneticisi tarafından atanır)
        public Yol Yol { get; set; }

        public Point Konum
        {
            get { return konum; }
            private set { konum = value; }
        }

        public int Hasar => hasar;
        public int Menzil => menzil;
        public double Hiz => hiz;
        public int Fiyat => fiyat;

        protected Kule(Point konum)
        {
            Konum = konum;
            sonSaldiriZamani = DateTime.MinValue;
        }

        // Her kule bunu kendine göre override edecek (Polymorphism)
        public abstract void Saldir(List<Dusman> dusmanlar);

        // Basit bir yükseltme mantığı – istersen sonra özelleştiririz
        public virtual void Yukselt()
        {
            hasar += 5;
            menzil += 10;
        }

        // Saldırı hızına göre cooldown kontrolü
        protected bool SaldiriHazirMi()
        {
            if (hiz <= 0)
                return true;

            double gerekenSure = 1.0 / hiz; // ör: hiz = 1 ise 1 sn, hiz = 2 ise 0.5 sn
            var gecenSure = (DateTime.Now - sonSaldiriZamani).TotalSeconds;

            if (gecenSure >= gerekenSure)
            {
                sonSaldiriZamani = DateTime.Now;
                return true;
            }

            return false;
        }

        // Menzil kontrolü için ortak fonksiyon
        protected bool MenziIcindemi(Dusman dusman)
        {
            return dusman.Mesafe(Konum) <= menzil;
        }
    }
}