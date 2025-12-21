using KuleSavunmaOyunu.Entities;
using KuleSavunmaOyunu.Enums;
using KuleSavunmaOyunu.Game;
using KuleSavunmaOyunu.Rendering;
using System.Drawing.Drawing2D;

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

            // Alt paneldeki kule butonlarini kart gibi stillendir (ikon + fiyat)
            KuleButonlariniStille();
            KonumlandirKuleButonlari();
            KuleButonFiyatRozetleriniKur();

            if (panelAlt != null)
                panelAlt.Resize += (s, e) => KonumlandirKuleButonlari();

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
            int w = panelOyunAlani.Width;
            int h = panelOyunAlani.Height;

            // Kenarlardan pay
            int margin = 60;

            // Güzergâhý oranlarla belirleyelim (panel boyutu deðiþse bile bozulmasýn)
            int yOrta = (int)(h * 0.45);  // soldan giriþ yüksekliði
            int yUst = (int)(h * 0.12);  // büyük U'nun tepe çizgisi
            int yAlt = (int)(h * 0.92);  // alt yatay (çok aþaðý deðmesin)
            int ySagUst = (int)(h * 0.55);  // saðdaki küçük U'nun üst çizgisi

            int xSolDonus = (int)(w * 0.25);  // soldaki büyük dönüþün x'i
            int xOrtaDikey = (int)(w * 0.62);  // ortadaki uzun dikey iniþ
            int xSagU = (int)(w * 0.78);  // saðdaki küçük U'nun sol dikeyi

            var noktalar = new List<Point>
    {
        // Soldan giriþ (yOrta)
        new Point(margin, yOrta),
        new Point(xSolDonus, yOrta),

        // Büyük U: yukarý çýk, saða git, aþaðý in
        new Point(xSolDonus, yUst),
        new Point(xOrtaDikey, yUst),
        new Point(xOrtaDikey, yAlt),

        // Alttan saða ilerle
        new Point(xSagU, yAlt),

        // Saðdaki küçük U: yukarý çýk ve saðdan çýkýþ yap
        new Point(xSagU, ySagUst),
        new Point(w - margin, ySagUst)
    };

            var yol = new Yol(noktalar);
            oyunYonetici = new OyunYonetici(yol);
            oyunYonetici.YeniDalgaBaslat();

            if (lblAltin != null) lblAltin.Text = oyunYonetici.Altin.ToString();
            if (lblCan != null) lblCan.Text = oyunYonetici.Can.ToString();
            if (lblDalga != null) lblDalga.Text = oyunYonetici.Dalga.ToString();
            if (lblSkor != null) lblSkor.Text = oyunYonetici.Skor.ToString();
        }


        private void KuleButonlariniStille()
        {
            StilVer(btnOkKulesi, KuleTipi.Ok, Color.SteelBlue);
            StilVer(btnTopKulesi, KuleTipi.Top, Color.OrangeRed);
            StilVer(btnBuyuKulesi, KuleTipi.Buyu, Color.MediumPurple);
            if (oyunYonetici == null)
            {
                if (btnOkKulesi != null) btnOkKulesi.Text = "Ok Kulesi";
                if (btnTopKulesi != null) btnTopKulesi.Text = "Top Kulesi";
                if (btnBuyuKulesi != null) btnBuyuKulesi.Text = "Büyü Kulesi";
            }
        }


        private void KuleButonMetinleriniGuncelle()
        {
            //rozet çiziyoruz
            if (btnOkKulesi != null) btnOkKulesi.Text = "Ok Kulesi";
            if (btnTopKulesi != null) btnTopKulesi.Text = "Top Kulesi";
            if (btnBuyuKulesi != null) btnBuyuKulesi.Text = "Büyü Kulesi";

            btnOkKulesi?.Invalidate();
            btnTopKulesi?.Invalidate();
            btnBuyuKulesi?.Invalidate();
        }


        private void KonumlandirKuleButonlari()
        {
            if (panelAlt == null || btnOkKulesi == null || btnTopKulesi == null || btnBuyuKulesi == null)
                return;

            int y = (panelAlt.Height - btnOkKulesi.Height) / 2;
            int padding = 40;

            btnOkKulesi.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnTopKulesi.Anchor = AnchorStyles.Bottom;
            btnBuyuKulesi.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            btnOkKulesi.Location = new Point(padding, y);
            btnTopKulesi.Location = new Point((panelAlt.Width - btnTopKulesi.Width) / 2, y);
            btnBuyuKulesi.Location = new Point(panelAlt.Width - btnBuyuKulesi.Width - padding, y);
        }

        private void StilVer(Button btn, KuleTipi tip, Color renk)
        {
            if (btn == null) return;

            btn.UseVisualStyleBackColor = false;
            btn.BackColor = renk;
            btn.ForeColor = Color.White;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;

            btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            btn.TextImageRelation = TextImageRelation.ImageAboveText;
            btn.ImageAlign = ContentAlignment.TopCenter;
            btn.TextAlign = ContentAlignment.BottomCenter;

            btn.Size = new Size(150, 85);

            btn.Padding = new Padding(0, 8, 0, 30);

            btn.Image = KuleIkonuOlustur(tip, 18, 18);

        }


        private Image KuleIkonuOlustur(KuleTipi tip, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush b = new SolidBrush(Color.FromArgb(220, Color.White)))
            using (Pen p = new Pen(Color.FromArgb(240, Color.White), 2))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                Rectangle r = new Rectangle(2, 2, w - 4, h - 4);

                if (tip == KuleTipi.Ok)
                {
                    Point[] tri =
                    {
                new Point(r.Left + r.Width / 2, r.Top),
                new Point(r.Right, r.Bottom),
                new Point(r.Left, r.Bottom)
            };
                    g.FillPolygon(b, tri);
                }
                else if (tip == KuleTipi.Top)
                {
                    g.FillEllipse(b, r);
                    g.DrawLine(p, r.Left + r.Width / 2, r.Top, r.Left + r.Width / 2, r.Top - 4);
                }
                else if (tip == KuleTipi.Buyu)
                {
                    Point[] diamond =
                    {
                new Point(r.Left + r.Width / 2, r.Top),
                new Point(r.Right, r.Top + r.Height / 2),
                new Point(r.Left + r.Width / 2, r.Bottom),
                new Point(r.Left, r.Top + r.Height / 2)
            };
                    g.FillPolygon(b, diamond);
                }
            }

            return bmp;
        }
        private void KuleButonFiyatRozetleriniKur()
        {
            if (btnOkKulesi != null) btnOkKulesi.Paint += (s, e) => CizFiyatRozeti((Button)s, e.Graphics, KuleTipi.Ok);
            if (btnTopKulesi != null) btnTopKulesi.Paint += (s, e) => CizFiyatRozeti((Button)s, e.Graphics, KuleTipi.Top);
            if (btnBuyuKulesi != null) btnBuyuKulesi.Paint += (s, e) => CizFiyatRozeti((Button)s, e.Graphics, KuleTipi.Buyu);
        }

        private void CizFiyatRozeti(Button btn, Graphics g, KuleTipi tip)
        {
            if (oyunYonetici == null) return;

            int fiyat = oyunYonetici.KuleFiyatiGetir(tip);
            string text = fiyat.ToString();

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Rozet fontunu biraz küçültmek daha þýk durur (opsiyonel)
            using var rozetFont = new Font(btn.Font.FontFamily, btn.Font.Size - 1f, FontStyle.Bold);

            Size t = TextRenderer.MeasureText(g, text, rozetFont, Size.Empty, TextFormatFlags.NoPadding);

            int padX = 10, padY = 6;
            int w = t.Width + padX * 2;
            int h = t.Height + padY;

            int x = (btn.ClientSize.Width - w) / 2;

            int badgeAreaTop = btn.ClientSize.Height - btn.Padding.Bottom;
            int badgeAreaHeight = btn.Padding.Bottom;
            int y = badgeAreaTop + (badgeAreaHeight - h) / 2;


            Rectangle r = new Rectangle(x, y, w, h);

            bool yetiyor = oyunYonetici.Altin >= fiyat;
            Color bg = yetiyor ? Color.FromArgb(230, Color.White) : Color.FromArgb(120, Color.White);
            Color fg = Color.Black;

            using (var path = YuvarlakDikdortgen(r, 10))
            using (var b = new SolidBrush(bg))
            using (var p = new Pen(Color.FromArgb(80, Color.Black), 1))
            {
                g.FillPath(b, path);
                g.DrawPath(p, path);
            }

            TextRenderer.DrawText(g, text, rozetFont, r, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }


        private GraphicsPath YuvarlakDikdortgen(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
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
                {
                    // Eðer maksimum kule sayýsýna ulaþýldýysa, kullanýcýya açýkça belirt
                    if (oyunYonetici.Kuleler.Count >= oyunYonetici.MaksimumKuleSayisi)
                        lblDurum.Text = $"Maksimum kule sayýsýna ulaþýldý ({oyunYonetici.MaksimumKuleSayisi}).";
                    else
                        lblDurum.Text = "Kule yerleþtirilemedi.";
                }
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
            btnOkKulesi?.Invalidate();
            btnTopKulesi?.Invalidate();
            btnBuyuKulesi?.Invalidate();

            panelOyunAlani.Invalidate();
        }

        private void lblDurum_Click(object sender, EventArgs e)
        {

        }
    }
}