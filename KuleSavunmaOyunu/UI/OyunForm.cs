using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KuleSavunmaOyunu.Game;
using KuleSavunmaOyunu.Rendering;
using KuleSavunmaOyunu.Entities;
using KuleSavunmaOyunu.Enums;

namespace KuleSavunmaOyunu.UI
{
    public partial class OyunForm : Form
    {
        // OYUN ALANI ÝÇÝN ALANLAR
        private OyunYonetici oyunYonetici;
        private CizimMotoru cizimMotoru;
        private KuleTipi? seciliKuleTipi = null;

        public OyunForm()
        {
            InitializeComponent();

            // Form'un kendisinde flicker azaltma
            this.DoubleBuffered = true;

            // panelOyunAlani için DoubleBuffered açýyoruz (reflection ile)
            if (panelOyunAlani != null)
            {
                panelOyunAlani.GetType()
                    .GetProperty("DoubleBuffered",
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.NonPublic)
                    ?.SetValue(panelOyunAlani, true, null);
            }

            cizimMotoru = new CizimMotoru();
        }

        // Form yüklendiðinde çalýþacak
        private void OyunForm_Load(object sender, EventArgs e)
        {
            // Yol oluþtur ve oyun yöneticisini baþlat
            YolOlusturVeOyunuBaslat();

            // Timer ayarlarý (Designer'da Enabled=false ise buradan baþlatýr)
            if (timerOyun != null)
            {
                timerOyun.Interval = 30; // ~33 FPS
                timerOyun.Start();
            }

            if (lblDurum != null)
                lblDurum.Text = "Oyun baþladý. Kule seçip oyun alanýna týkla.";
        }

        private void YolOlusturVeOyunuBaslat()
        {
            // panelOyunAlani'nýn boyutuna göre örnek bir yol
            int genislik = panelOyunAlani.Width;
            int yukseklik = panelOyunAlani.Height;

            var noktalar = new List<Point>
            {
                new Point(50, yukseklik / 4),
                new Point(genislik / 2, yukseklik / 4),
                new Point(genislik / 2, yukseklik - 150),
                new Point(genislik - 50, yukseklik - 150)
            };

            var yol = new Yol(noktalar);

            oyunYonetici = new OyunYonetici(yol);
            oyunYonetici.YeniDalgaBaslat(); // Ýlk dalgayý baþlat

            // Baþlangýç deðerlerini label'lara yaz
            if (lblAltin != null) lblAltin.Text = oyunYonetici.Altin.ToString();
            if (lblCan != null) lblCan.Text = oyunYonetici.Can.ToString();
            if (lblDalga != null) lblDalga.Text = oyunYonetici.Dalga.ToString();
            if (lblSkor != null) lblSkor.Text = oyunYonetici.Skor.ToString();
        }

        // OYUN ALANI ÇÝZÝMÝ
        private void panelOyunAlani_Paint(object sender, PaintEventArgs e)
        {
            if (oyunYonetici == null || cizimMotoru == null)
                return;

            cizimMotoru.Ciz(e.Graphics, oyunYonetici);
        }

        // OYUN ALANINA TIKLAYINCA KULE KOYMA
        private void panelOyunAlani_MouseClick(object sender, MouseEventArgs e)
        {
            if (oyunYonetici == null)
                return;

            if (!seciliKuleTipi.HasValue)
            {
                if (lblDurum != null)
                    lblDurum.Text = "Önce alttan bir kule seç.";
                return;
            }

            bool basarili = oyunYonetici.KuleKoy(e.Location, seciliKuleTipi.Value);

            if (!basarili)
            {
                if (lblDurum != null)
                    lblDurum.Text = "Kule yerleþtirilemedi (yetersiz altýn veya çok yakýn).";
            }
            else
            {
                if (lblDurum != null)
                    lblDurum.Text = "Kule yerleþtirildi.";
            }

            // Hemen yeniden çiz
            panelOyunAlani.Invalidate();
        }

        // KULE SEÇÝM BUTONLARI
        private void btnOkKulesi_Click(object sender, EventArgs e)
        {
            seciliKuleTipi = KuleTipi.Ok;
            if (lblDurum != null)
                lblDurum.Text = "Ok Kulesi seçildi.";
        }

        private void btnTopKulesi_Click(object sender, EventArgs e)
        {
            seciliKuleTipi = KuleTipi.Top;
            if (lblDurum != null)
                lblDurum.Text = "Top Kulesi seçildi.";
        }

        private void btnBuyuKulesi_Click(object sender, EventArgs e)
        {
            seciliKuleTipi = KuleTipi.Buyu;
            if (lblDurum != null)
                lblDurum.Text = "Büyü Kulesi seçildi.";
        }

        // Alt panelin Paint event'i þu an kullanýlmýyor, boþ kalabilir
        private void panelAlt_Paint(object sender, PaintEventArgs e)
        {
        }

        // OYUN DÖNGÜSÜ (her timer tick'inde)
        private void timerOyun_Tick(object sender, EventArgs e)
        {
            if (oyunYonetici == null)
                return;

            oyunYonetici.Guncelle();

            // Label güncellemeleri
            if (lblAltin != null) lblAltin.Text = oyunYonetici.Altin.ToString();
            if (lblCan != null) lblCan.Text = oyunYonetici.Can.ToString();
            if (lblDalga != null) lblDalga.Text = oyunYonetici.Dalga.ToString();
            if (lblSkor != null) lblSkor.Text = oyunYonetici.Skor.ToString();

            if (oyunYonetici.OyunBittiMi())
            {
                timerOyun.Stop();
                if (lblDurum != null)
                    lblDurum.Text = "Oyun bitti!";
                MessageBox.Show("Oyun bitti! Skor: " + oyunYonetici.Skor, "Game Over");
            }

            // Oyun alanýný yeniden çiz
            panelOyunAlani.Invalidate();
        }

        private void lblDurum_Click(object sender, EventArgs e)
        {

        }
    }
}
