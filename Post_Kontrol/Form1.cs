using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Diagnostics;
using Tulpep.NotificationWindow;
namespace Post_Kontrol
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            notifyIcon1.BalloonTipTitle = "İpucu - Post Kontrol Programı";
            notifyIcon1.BalloonTipText = "Metin kutularına veya url listesine çift tıklayarak renk menüsünü açabilirsiniz.";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ShowBalloonTip(2000);
          
        }

        public void _2indirme_bitti(object sender, DownloadStringCompletedEventArgs e)
        {
            try {

            timer1.Stop(); 
            HtmlAgilityPack.HtmlDocument _dokuman_ = new HtmlAgilityPack.HtmlDocument();
            _dokuman_.LoadHtml(e.Result);
            HtmlNodeCollection _post_icerigi_ = _dokuman_.DocumentNode.SelectNodes("//div[contains(@class, 'postbit-new-message')]");
            HtmlNodeCollection _uye_nick_adi_ = _dokuman_.DocumentNode.SelectNodes("//div[contains(@class, 'postbit-new-username')]");

            for (int i = 0; i < _post_icerigi_.Count; i++)
            {
                    post_idsi = _post_icerigi_[i].Id.Substring(_post_icerigi_[i].Id.LastIndexOf("_") + 1);

                    for (int h = 0; h < kelime_listesi.Length; h++)
                      {
                       
                        if (_post_icerigi_[i].InnerText.ToLower().Contains(kelime_listesi[h].ToLower()) && !_kontrolled.Contains(post_idsi))
                        {

                        var = true;
                        if (checkBox1.Checked)
                        {
                           
                                PopupNotifier pf = new PopupNotifier();
                                pf.Click += new EventHandler(notifyIcon1_BalloonTipClicked);
                                pf.Image = Image.FromFile("ico.ico");
                                pf.ImageSize = new Size(32, 32);
                                pf.TitleText = "Filtrelenmiş Kelime Bulundu";
                                pf.ContentText =
                                "Bulunan Kelime: " + kelime_listesi[h] + "\n" +
                                "Kullanıcı Adı: " + _uye_nick_adi_[i].InnerText +
                                "\n" + "Post ID: " + post_idsi +
                                "\nKonu-Post Linki: " + listBox1.SelectedItem.ToString() + "#post" + post_idsi;
                                url2 = listBox1.SelectedItem.ToString()  +"#post" + post_idsi;
                                pf.Popup();

                        }
                        if (checkBox2.Checked)
                        {
                            if (!string.IsNullOrEmpty(url_ses))
                            {
                                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(url_ses);
                                sp.Play();
                            }

                        }
                        if (checkBox3.Checked)
                        {
                          
                            using (StreamWriter sw = File.AppendText("log-post.log"))
                            {
                                sw.WriteLine("Bulunan Kelime: " + kelime_listesi[h] + "\n" +
                               "Kullanıcı Adı: " + _uye_nick_adi_[i].InnerText +
                               "\n" + "Post ID: " + post_idsi +
                               "\nKonu-Post Linki: " + listBox1.SelectedItem.ToString() + "#post" + post_idsi + "\n\n");
                            }
                            
                            }

                            _kontrolled += post_idsi;
                    }
                    else
                    {
                        var = false;
                    }

                    }
                if (var)
                {

                    richTextBox1.Text += "\n\n----------------------------------------------------------------------------------------Post Alanı:----------------------------------------------------------------------------------------\n" + "Post Sahibi Kullanıcı Adı: " + _uye_nick_adi_[i].InnerText + "\n\n" + _post_icerigi_[i].InnerText + "\n\nPost ID: " + _post_icerigi_[i].Id.Substring(_post_icerigi_[i].Id.LastIndexOf("_") + 1) + "\n\nFiltreli kelime Algılandı, Post ID'si: " + _post_icerigi_[i].Id.Substring(_post_icerigi_[i].Id.LastIndexOf("_") + 1) + "\n--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n";

                }
                else
                {
                    richTextBox1.Text += "\n\n----------------------------------------------------------------------------------------Post Alanı:----------------------------------------------------------------------------------------\n" + "Post Sahibi Kullanıcı Adı: " + _uye_nick_adi_[i].InnerText + "\n\n" + _post_icerigi_[i].InnerText + "\n\nPost ID: " + _post_icerigi_[i].Id.Substring(_post_icerigi_[i].Id.LastIndexOf("_") + 1) + "\n-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n";
                    richTextBox1.Text = richTextBox1.Text.Replace("Al&#305;nt&#305", "Alıntı")
                    .Replace("    ", "====>")
                    .Replace("====>====>====>====>", string.Empty)
                    .Replace("====>====>====>", string.Empty)
                    .Replace("====> ", " ")
                    .Replace("					  ", "Alıntı içeriği: ")
                    .Replace(" Alıntı", "Alıntı");
                }
                  
               }

                richTextBox1.Text = richTextBox1.Text.Replace("====>", "\n\nPost İçeriği====>");
                timer1.Start();
            }
            catch (Exception ex) { button2.Enabled = true; button3.Enabled = false; MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
        }
        bool var = false;
        string[] kelime_listesi = new string[] { "X" };
        string konu_Adresi = string.Empty;
        string post_idsi = string.Empty;
        string url_ses = string.Empty;
        string _kontrolled = "";
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                 richTextBox1.Text = string.Empty;
                if(!textBox1.Text.Contains(","))
                {
                    kelime_listesi[0] = textBox1.Text;
                }
                else { kelime_listesi = textBox1.Text.Split(','); }
                 Uri url = new Uri(konu_Adresi);
                 WebClient _wc_ = new WebClient();
                _wc_.Headers.Add("User-Agent: Other"); //yasak hatasını çözmek için.
                _wc_.Encoding = Encoding.Default;
                _wc_.DownloadStringCompleted += new DownloadStringCompletedEventHandler(_2indirme_bitti);
                _wc_.DownloadStringAsync(url);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message,"Hata",MessageBoxButtons.OK,MessageBoxIcon.Exclamation); }

        }

        int _substring_baslangici = 0;
        public void mesajlari_cek(string e)
        {
            try
            {
                int a = 0;
                HtmlAgilityPack.HtmlDocument _dokuman_ = new HtmlAgilityPack.HtmlDocument();
                _dokuman_.LoadHtml(e);
                foreach (HtmlNode link in _dokuman_.DocumentNode.SelectNodes("//a[@href]"))
                {
                    a = a + 1;

                    try
                    {
                        switch (comboBox1.SelectedIndex)
                        {
                        case 0: //Yardım merkezi
                                if (link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("yardim-merkezi")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc")
                                    && a > 77 && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
                                    && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring(_substring_baslangici).Contains("-")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php"))
                                    {
                                    if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
                                    {
                                        listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
                                    }
                                }
                        break;

                            case 1: //sosyal medya bölümü
                                try
                                {
                                    if (!link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc")  
                                    && a > 97 && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring(64).Contains("-") && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("members")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php"))
                                    {
                                        if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
                                        {
                                            listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
                                        }

                                    }
                                }
                                catch (Exception) { }
                                break;

                            case 2:  //Android bölümü
                                try
                                {
                                    if (!link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc")
                                    && a > 216 && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring(_konu_urlsi.Length).Contains("-") && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("members")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php"))
                                    {
                                        if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
                                        {
                                            listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
                                        }

                                    }
                                }
                                catch (Exception) { }
                            break;

                            case 3: //Grafik tasarım bölümü
                                try
                                {
                                    if (!link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc") 
                                    && a > 90 && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring(_substring_baslangici).Contains("-") && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("members")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php"))
                                    {
                                        if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
                                        {
                                            listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
                                        }

                                    }
                                }
                                catch (Exception) { }
                                break;

                            case 4:  //Network bölümü
                                try
                                {
                                    if (!link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc")
                                    && a > 140 && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring(_substring_baslangici).Contains("-") && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("members")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("pomera")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#information"))
                                    {
                                        if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
                                        {
                                            listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
                                        }

                                    }
                                }
                                catch (Exception) { }
                                break;

                            case 5:  //Web-Server
                                try
                                {
                                    if (!link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc")
                                    && a > 115 && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring(_substring_baslangici).Contains("-") && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("members")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("pomera")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#information"))
                                    {
                                        if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
                                        {
                                            listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
                                        }

                                    }
                                }
                                catch (Exception) { }
                                break;

                            case 6:   //Off-Topic
                                try
                                {
                                    if (!link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc")
                                    && a > 120 && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring(_substring_baslangici).Contains("-") && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("members")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("pomera")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#information"))
                                    {
                                        if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
                                        {
                                            listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
                                        }

                                    }
                                }
                                catch (Exception) { }
                                break;
                            case 7:   //Trojan ve virüsler
                                try
                                {
                                    if (!link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc")
                                    && a > 115 && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring(_substring_baslangici).Contains("-") && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("members")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("pomera")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#information")
                                    && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#turkhackteam-under-ground"))
                                    {
                                        if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
                                        {
                                            listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
                                        }

                                    }
                                }
                                catch (Exception) { }
                                break;
                        }
                        
                    }
                    catch (Exception) { }

                  label1.Text = "Konu Adresleri ve Sayfa Sayısı ["+comboBox1.SelectedItem.ToString() + "]: " + listBox1.Items.Count.ToString();
                
                 if (checkBox4.Checked)
                 {
                        
                       for (int l = 0; l < listBox1.Items.Count; l++)
                       {

                            if (comboBox1.SelectedIndex == 0)
                            {
                                string worts = "abcdfgğhıijkmnprstuüvyz";   //açmak kelimesinden türetilmiş kelimelerde de "acil" kelimesi bulunduğu için basit bir filtreleme işlemi yapıyoruz uyarı vermemesi için.
                                for (int m = 0; m < worts.Length - 1; m++)
                                {
                                    if (listBox1.Items[l].ToString().Contains("acil" + worts[m].ToString()))
                                    {
                                        listBox1.Items[l] = listBox1.Items[l].ToString()
                                        .Replace("acil" + worts[m], "açıl" + worts[m]);
                                    }
                                }

                            }

                         string num = listBox1.Items[l].ToString().Substring(_substring_baslangici, 7);
                         if (!aytimler.Contains(num))
                         {
                             for (int c = 0; c < baslik.Length; c++)
                             {
                                 if (listBox1.Items[l].ToString().Substring(_substring_baslangici + 7 + 1).ToLower().Contains(baslik[c].ToLower().Replace("ç", "c").Replace("ğ", "g").Replace("ş", "s").Replace("ö", "o").Replace("ü", "u").Replace("ı", "i")))
                                 {
                                    if (checkBox3.Checked) {

                                            using (StreamWriter sw = File.AppendText("log-konu-basligi.log"))
                                            {
                                                sw.WriteLine("Kelime: " + baslik[c] + "\nURL: " + listBox1.Items[l].ToString().Replace("ç", "c").Replace("ı", "i"));
                                            }
                                        }
                                     say = say + 1;
                                     System.Threading.Thread.Sleep(150);
                                     PopupNotifier pf = new PopupNotifier();
                                     pf.Click += new EventHandler(notifyIcon1_BalloonTipClicked);
                                     pf.Image = Image.FromFile("ico.ico");
                                     pf.ImageSize = new Size(32, 32);
                                     pf.TitleText = "Konuda Başlığında Gereksiz Kelime Bulundu";
                                     pf.ContentText =
                                     "Bulunan Kelime: " + baslik[c] +"\nGereksiz kelime içeren konu sayısı: "+ say.ToString() +
                                     "\nKonu Linki: " + listBox1.Items[l].ToString().Replace("ç", "c").Replace("ı", "i");
                                     url2 = listBox1.Items[l].ToString().Replace("ç", "c").Replace("ı", "i");
                                     pf.Popup();
                                     aytimler += num;
                                     if (checkBox2.Checked)
                                     {
                                         if (!string.IsNullOrEmpty(url_ses))
                                         {
                                             System.Media.SoundPlayer sp = new System.Media.SoundPlayer(url_ses);
                                             sp.Play();
                                         }

                                     }
                                 }
                             }

                         }
                     }
                       
                   }
                 
                }
                timer1.Enabled = true;
                timer1.Start();
                say = 0;
            }
            catch (Exception ex) { button2.Enabled = true; button3.Enabled = false; MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
        }

        string _konu_urlsi = null;
        string[] baslik = new string[] { "X" };
        int say = 0;


        public void _wc_indirme_islemi_bitti(object sender, DownloadStringCompletedEventArgs e)
        {

            try
            {
                mesajlari_cek(e.Result);

                /*
                 * //Test alanı
                int a = 0;
                HtmlAgilityPack.HtmlDocument _dokuman_ = new HtmlAgilityPack.HtmlDocument();
                _dokuman_.LoadHtml(e.Result);
                foreach (HtmlNode link in _dokuman_.DocumentNode.SelectNodes("//a[@href]"))
                {
                    a = a + 1;
                   
                    try
                    {
                        if (!link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc")  //Network bölümü
                        && a > 115 && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring("https://www.turkhackteam.org/trojan-ve-virusler/".Length).Contains("-") && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
                        && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("members")
                        && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php")
                        && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("pomera")
                        && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#information")
                        && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#turkhackteam-under-ground"))
                        {
                            if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
                            {
                                listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
                            }

                        }
                    }
                    catch (Exception) { }
                }
               */
            }
            catch (Exception ex) { button2.Enabled = true; button3.Enabled = false; MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
        }


        int b = 0;
        string aytimler = string.Empty;
        private void button2_Click(object sender, EventArgs e)
        {
          if (comboBox1.SelectedIndex > -1)
            {
           button2.Enabled = false;
           button3.Enabled = true;
           timer1.Interval = Convert.ToInt32(numericUpDown1.Value * 1000);
           listBox1.Items.Clear();
           if (!textBox2.Text.Contains(","))
           {
              baslik[0] = textBox2.Text;
           }
           else { baslik = textBox2.Text.Split(','); }
           richTextBox1.Text = string.Empty;
            Uri url = new Uri(_konu_urlsi);
            //Uri url = new Uri("https://www.turkhackteam.org/trojan-ve-virusler/"); test içindi.
            WebClient _wc_ = new WebClient();
           _wc_.Headers.Add("User-Agent: Other");
           _wc_.Encoding = Encoding.Default;
           _wc_.DownloadStringCompleted += new DownloadStringCompletedEventHandler(_wc_indirme_islemi_bitti);
           _wc_.DownloadStringAsync(url);

            }
            else { MessageBox.Show("Lütfen bir kategori seçin.","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Exclamation); return; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {

                listBox1.SelectedIndex = b;
                richTextBox1.Text = string.Empty;
                konu_Adresi = listBox1.SelectedItem.ToString().Replace("ç","c").Replace("ı", "i");
                button1.PerformClick();
                b = b + 1;
              
            }
            catch (Exception) {
                button2.Enabled = true;
                button2.PerformClick();
                b = 0;
                button2.Enabled = false;
                timer1.Stop();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Hakkinda().Show();
        }

        private void gizleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
        }
        string url2 = string.Empty;
        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {

            try
            {
                Process.Start(url2);
            }
            catch (Exception) { }
          
        }

        private void hakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Hakkinda().Show();
        }

        private void türkhackteamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.turkhackteam.org/");
        }

        private void başlatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2.PerformClick();
            başlatToolStripMenuItem.Enabled = false;
            durdurToolStripMenuItem.Enabled = true;
        }

        private void durdurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button3.PerformClick();
            durdurToolStripMenuItem.Enabled = false;
            başlatToolStripMenuItem.Enabled = true;
        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Bir ses dosyası seç.";
                op.Filter = "wav dosyası|*.wav";
                op.Multiselect = false;
                if (op.ShowDialog() == DialogResult.OK)
                {
                    url_ses = op.FileName;
                }
                else { checkBox2.Checked = false; }
            }
            
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try {
                if (File.Exists("log-post.log")) { Process.Start("notepad.exe", Environment.CurrentDirectory + @"\log-post.log"); }
                if (File.Exists("log-konu-basligi.log")) { Process.Start("notepad.exe", Environment.CurrentDirectory + @"\log-konu-basligi.log"); }
            }
            catch (Exception) { }
            
        }
        string def = "Ellerinize sağlık,ellerine sağlık,yüreğine sağlık,güzel olmuş";
        string def2 = "lütfen,yardım,acil";
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _kontrolled = string.Empty;
            post_idsi = string.Empty;
            url2 = string.Empty;
            url_ses = string.Empty;
            textBox1.Text = def;
            listBox1.Items.Clear();
            richTextBox1.Text = string.Empty;
            textBox2.Text = def2;
            aytimler = string.Empty;
            b = 0;
        }

        private void kaldırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
                listBox1.Items.Remove(listBox1.SelectedItem);
            
        }

        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if(cd.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.BackColor = cd.Color;
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                listBox1.BackColor = cd.Color;
            }
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                this.BackColor = cd.Color;
            }
        }

        private void textBox2_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                textBox2.BackColor = cd.Color;
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                textBox1.BackColor = cd.Color;
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox1.BackColor = SystemColors.ButtonShadow;
            textBox1.BackColor = SystemColors.ButtonShadow;
            textBox2.BackColor = SystemColors.ButtonShadow;
            listBox1.BackColor = SystemColors.ButtonShadow;
            this.BackColor = SystemColors.ButtonShadow;
        }

        private void seçiliAdreseGitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
                Process.Start(listBox1.SelectedItem.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
               _konu_urlsi = "https://www.turkhackteam.org/tht-yardim-merkezi/";
              _substring_baslangici = _konu_urlsi.Length;
               textBox2.Text = "Acil,lütfen,yardım";
                break;
                case 1:
               _konu_urlsi = "https://www.turkhackteam.org/sosyal-medya-ve-sosyal-platformlar/";
               _substring_baslangici = _konu_urlsi.Length;
               textBox2.Text = "Filtrelenicek kelime(ler)/cümle(ler)";
               break;
               case 2:
               _konu_urlsi = "https://www.turkhackteam.org/google-android/";
               _substring_baslangici = _konu_urlsi.Length;
               textBox2.Text = "Filtrelenicek kelime(ler)/cümle(ler)";
               break;
               case 3:
              _konu_urlsi = "https://www.turkhackteam.org/grafik-tasarimi/";
              _substring_baslangici = _konu_urlsi.Length;
               textBox2.Text = "Filtrelenicek kelime(ler)/cümle(ler)";
               break;
               case 4:
              _konu_urlsi = "https://www.turkhackteam.org/network/";
              _substring_baslangici = _konu_urlsi.Length;
              textBox2.Text = "Filtrelenicek kelime(ler)/cümle(ler)";
               break;
               case 5:
               _konu_urlsi = "https://www.turkhackteam.org/web-server-guvenligi/";
               _substring_baslangici = _konu_urlsi.Length;
               textBox2.Text = "Filtrelenicek kelime(ler)/cümle(ler)";
               break;
               case 6:
              _konu_urlsi = "https://www.turkhackteam.org/off-topic/";
              _substring_baslangici = _konu_urlsi.Length;
               textBox2.Text = "Nasıl hacker olurum";
               break;
               case 7:
              _konu_urlsi = "https://www.turkhackteam.org/trojan-ve-virusler/";
              _substring_baslangici = _konu_urlsi.Length;
               textBox2.Text = "Filtrelenicek kelime(ler)/cümle(ler)";
              break;
            }
            label9.Text = "Seçili Kategori: " + comboBox1.SelectedItem.ToString();
            b = 0;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

//https://www.youtube.com/watch?v=akmZj4CyPkw  


/*
  try
  {
  if (!link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("misc")  //sosyal medya bölümü
  && a > 97 && link.GetAttributeValue("href", "Gösterilecek veri yok.").Substring(64).Contains("-") && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("#post")
  && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("members")
  && !link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("forumdisplay.php")) //.../index2.html gibi adresleri çekmemesi için.
         {
             if (!listBox1.Items.Contains(link.GetAttributeValue("href", "Gösterilecek veri yok.")))
             {
                 listBox1.Items.Add(link.GetAttributeValue("href", "Gösterilecek veri yok."));
             }

         }
     }
     catch (Exception) { }

 }
 */
