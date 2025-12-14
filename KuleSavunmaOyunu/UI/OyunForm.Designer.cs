using System.Windows.Forms;


namespace KuleSavunmaOyunu.UI
{
    partial class OyunForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelUst = new Panel();
            lblSkorBaslik = new Label();
            lblDalgaBaslik = new Label();
            lblCanBaslik = new Label();
            lblAltinBaslik = new Label();
            lblDurum = new Label();
            lblSkor = new Label();
            lblDalga = new Label();
            lblCan = new Label();
            lblAltin = new Label();
            panelAlt = new Panel();
            btnBuyuKulesi = new Button();
            btnTopKulesi = new Button();
            btnOkKulesi = new Button();
            panelOyunAlani = new Panel();
            timerOyun = new System.Windows.Forms.Timer(components);
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            panelUst.SuspendLayout();
            panelAlt.SuspendLayout();
            SuspendLayout();
            // 
            // panelUst
            // 
            panelUst.BackColor = SystemColors.ActiveBorder;
            panelUst.Controls.Add(lblSkorBaslik);
            panelUst.Controls.Add(lblDalgaBaslik);
            panelUst.Controls.Add(lblCanBaslik);
            panelUst.Controls.Add(lblAltinBaslik);
            panelUst.Controls.Add(lblDurum);
            panelUst.Controls.Add(lblSkor);
            panelUst.Controls.Add(lblDalga);
            panelUst.Controls.Add(lblCan);
            panelUst.Controls.Add(lblAltin);
            panelUst.Dock = DockStyle.Top;
            panelUst.Location = new Point(0, 0);
            panelUst.Name = "panelUst";
            panelUst.Size = new Size(1205, 60);
            panelUst.TabIndex = 0;
            // 
            // lblSkorBaslik
            // 
            lblSkorBaslik.AutoSize = true;
            lblSkorBaslik.Font = new Font("Stencil", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSkorBaslik.Location = new Point(604, 2);
            lblSkorBaslik.Name = "lblSkorBaslik";
            lblSkorBaslik.Size = new Size(47, 18);
            lblSkorBaslik.TabIndex = 8;
            lblSkorBaslik.Text = "SKOR";
            // 
            // lblDalgaBaslik
            // 
            lblDalgaBaslik.AutoSize = true;
            lblDalgaBaslik.Font = new Font("Stencil", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDalgaBaslik.Location = new Point(439, 2);
            lblDalgaBaslik.Name = "lblDalgaBaslik";
            lblDalgaBaslik.Size = new Size(53, 18);
            lblDalgaBaslik.TabIndex = 7;
            lblDalgaBaslik.Text = "DALGA";
            // 
            // lblCanBaslik
            // 
            lblCanBaslik.AutoSize = true;
            lblCanBaslik.Font = new Font("Stencil", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCanBaslik.Location = new Point(246, 4);
            lblCanBaslik.Name = "lblCanBaslik";
            lblCanBaslik.Size = new Size(36, 18);
            lblCanBaslik.TabIndex = 6;
            lblCanBaslik.Text = "CAN";
            // 
            // lblAltinBaslik
            // 
            lblAltinBaslik.AutoSize = true;
            lblAltinBaslik.Font = new Font("Stencil", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAltinBaslik.Location = new Point(65, 4);
            lblAltinBaslik.Name = "lblAltinBaslik";
            lblAltinBaslik.Size = new Size(50, 18);
            lblAltinBaslik.TabIndex = 5;
            lblAltinBaslik.Text = "ALTIN";
            // 
            // lblDurum
            // 
            lblDurum.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblDurum.AutoSize = true;
            lblDurum.ForeColor = Color.White;
            lblDurum.Location = new Point(1034, 22);
            lblDurum.Name = "lblDurum";
            lblDurum.Size = new Size(54, 20);
            lblDurum.TabIndex = 4;
            lblDurum.Text = "Durum";
            lblDurum.Click += lblDurum_Click;
            // 
            // lblSkor
            // 
            lblSkor.AutoSize = true;
            lblSkor.Location = new Point(604, 22);
            lblSkor.Name = "lblSkor";
            lblSkor.Size = new Size(38, 20);
            lblSkor.TabIndex = 3;
            lblSkor.Text = "Skor";
            // 
            // lblDalga
            // 
            lblDalga.AutoSize = true;
            lblDalga.Location = new Point(439, 22);
            lblDalga.Name = "lblDalga";
            lblDalga.Size = new Size(49, 20);
            lblDalga.TabIndex = 2;
            lblDalga.Text = "Dalga";
            // 
            // lblCan
            // 
            lblCan.AutoSize = true;
            lblCan.Location = new Point(246, 22);
            lblCan.Name = "lblCan";
            lblCan.Size = new Size(34, 20);
            lblCan.TabIndex = 1;
            lblCan.Text = "Can";
            lblCan.Click += lblCan_Click;
            // 
            // lblAltin
            // 
            lblAltin.AutoSize = true;
            lblAltin.Location = new Point(65, 22);
            lblAltin.Name = "lblAltin";
            lblAltin.Size = new Size(40, 20);
            lblAltin.TabIndex = 0;
            lblAltin.Text = "Altın";
            lblAltin.Click += lblAltin_Click;
            // 
            // panelAlt
            // 
            panelAlt.BackColor = SystemColors.ControlDark;
            panelAlt.Controls.Add(label3);
            panelAlt.Controls.Add(label2);
            panelAlt.Controls.Add(label1);
            panelAlt.Controls.Add(btnBuyuKulesi);
            panelAlt.Controls.Add(btnTopKulesi);
            panelAlt.Controls.Add(btnOkKulesi);
            panelAlt.Dock = DockStyle.Bottom;
            panelAlt.Location = new Point(0, 608);
            panelAlt.Name = "panelAlt";
            panelAlt.Size = new Size(1205, 100);
            panelAlt.TabIndex = 1;
            panelAlt.Paint += panelAlt_Paint;
            // 
            // btnBuyuKulesi
            // 
            btnBuyuKulesi.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnBuyuKulesi.Location = new Point(662, 37);
            btnBuyuKulesi.Name = "btnBuyuKulesi";
            btnBuyuKulesi.Size = new Size(94, 29);
            btnBuyuKulesi.TabIndex = 2;
            btnBuyuKulesi.Text = "Büyü Kulesi";
            btnBuyuKulesi.UseVisualStyleBackColor = true;
            btnBuyuKulesi.Click += btnBuyuKulesi_Click;
            // 
            // btnTopKulesi
            // 
            btnTopKulesi.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnTopKulesi.Location = new Point(395, 37);
            btnTopKulesi.Name = "btnTopKulesi";
            btnTopKulesi.Size = new Size(94, 29);
            btnTopKulesi.TabIndex = 1;
            btnTopKulesi.Text = "Top Kulesi";
            btnTopKulesi.UseVisualStyleBackColor = true;
            btnTopKulesi.Click += btnTopKulesi_Click;
            // 
            // btnOkKulesi
            // 
            btnOkKulesi.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOkKulesi.Location = new Point(105, 37);
            btnOkKulesi.Name = "btnOkKulesi";
            btnOkKulesi.Size = new Size(94, 29);
            btnOkKulesi.TabIndex = 0;
            btnOkKulesi.Text = "Ok Kulesi";
            btnOkKulesi.UseVisualStyleBackColor = true;
            btnOkKulesi.Click += btnOkKulesi_Click;
            // 
            // panelOyunAlani
            // 
            panelOyunAlani.BackColor = Color.Green;
            panelOyunAlani.Dock = DockStyle.Fill;
            panelOyunAlani.Location = new Point(0, 60);
            panelOyunAlani.Name = "panelOyunAlani";
            panelOyunAlani.Size = new Size(1205, 548);
            panelOyunAlani.TabIndex = 2;
            panelOyunAlani.Paint += panelOyunAlani_Paint;
            panelOyunAlani.MouseClick += panelOyunAlani_MouseClick;
            // 
            // timerOyun
            // 
            timerOyun.Enabled = true;
            timerOyun.Interval = 30;
            timerOyun.Tick += timerOyun_Tick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Stencil", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(126, 16);
            label1.Name = "label1";
            label1.Size = new Size(35, 18);
            label1.TabIndex = 6;
            label1.Text = "100";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Stencil", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(418, 16);
            label2.Name = "label2";
            label2.Size = new Size(35, 18);
            label2.TabIndex = 7;
            label2.Text = "250";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Stencil", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(679, 16);
            label3.Name = "label3";
            label3.Size = new Size(35, 18);
            label3.TabIndex = 8;
            label3.Text = "200";
            // 
            // OyunForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1205, 708);
            Controls.Add(panelOyunAlani);
            Controls.Add(panelAlt);
            Controls.Add(panelUst);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "OyunForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Kule Savunma Oyunu";
            Load += OyunForm_Load;
            panelUst.ResumeLayout(false);
            panelUst.PerformLayout();
            panelAlt.ResumeLayout(false);
            panelAlt.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelUst;
        private Label lblSkor;
        private Label lblDalga;
        private Label lblCan;
        private Label lblAltin;
        private Panel panelAlt;
        private Button btnBuyuKulesi;
        private Button btnTopKulesi;
        private Button btnOkKulesi;
        private Panel panelOyunAlani;
        private System.Windows.Forms.Timer timerOyun;
        private Label lblDurum;
        private Label lblSkorBaslik;
        private Label lblDalgaBaslik;
        private Label lblCanBaslik;
        private Label lblAltinBaslik;
        private Label label3;
        private Label label2;
        private Label label1;
    }
}
