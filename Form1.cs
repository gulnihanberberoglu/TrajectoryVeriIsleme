using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TrajectoryVerisiIsleme.WinForm
{
    public partial class Form1 : Form
    {
        //Dosyadan okunan kordinat değerlerini,Indirgenmiş kordinat değerlerini ve
        //Sorgulanmiş kordinat değerlerini Geolife türünde tutan listeler tanımlanıyor
        private readonly List<GeoLife> kordinatlar;

        private List<GeoLife> indirgenmisKordinatlar;

        private List<GeoLife> sorgulanmisKordinatlar;
        
        GMapOverlay routes;

        //Texboxlara girilen sunucuIp ve sunucuPort bilgisi alınıp sunucuIp ve sunucuPort değişkenlerine geçiliyor
        private string sunucuIp => toolStripTextBoxSunucuIp.Text;

        private string sunucuPort => toolStripTextBoxSunucuPort.Text;

        Graphics formGraphics;
        Rectangle rect;
        bool isDown = false;
        int initialX;
        int initialY;

        public Form1()
        {
            kordinatlar = new List<GeoLife>();
            sorgulanmisKordinatlar = new List<GeoLife>();
            indirgenmisKordinatlar = new List<GeoLife>();
            InitializeComponent();
            //haritada mause ile zoom yapmak için
            gMapControl.IgnoreMarkerOnMouseWheel = true;
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            //DataSeti dosyasını seçme işlemi
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //Winform projenin klasörünü bulur
            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.Filter = "txt dosyaları (* .txt) | * .txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            //Dosya seçilme işlemi gerçekleşmişse
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Dosya satır satır okunuyor
                var lines = File.ReadAllLines(openFileDialog1.FileName);
                int sayac = 0;
                foreach (var line in lines)
                {
                    //kordinatlar listesine dosyadan okunan noktalar verilir
                    var nokta = (GeoLife)line;
                    nokta.Adi = "Nokta" + sayac;
                    kordinatlar.Add(nokta);
                    sayac++;
                }
                //noktaları ve yolları haritaya çizen metot çağrılıyor
                kordinatCiz();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Form yüklendiğindeki harita ayarları gerçekleşiyor
            gMapControl.MapProvider = GMapProviders.GoogleMap;
            gMapControl.MinZoom = 0;
            gMapControl.MaxZoom = 24;
            gMapControl.Zoom = 9;
        }

        //noktaları ve yolları haritaya çizen metot
        private void kordinatCiz()
        {
            routes = new GMapOverlay();
            gMapControl.Overlays.Clear();
            noktalariCiz();
            yoluCiz();
            gMapControl.Overlays.Add(routes);
            //haritayı ilk nokta konumuna ortalamak için 
            gMapControl.Position = (PointLatLng)kordinatlar.First();
            //Haritanın son hali güncelleniyor
            gMapControl.ReloadMap();
        }

        //Yolların çizildiği metot
        private void yoluCiz()
        {
            //PointLatLng  türünde noktaların tutulacağı liste oluşturuluyor
            var noktalar = new List<PointLatLng>();
            //kordinatlar noktalar listesine ekleniyor
            foreach (var koordinat in kordinatlar)
            {
                noktalar.Add((PointLatLng)koordinat);
            }
            //Standart noktalar arası kırmızı yollar çiziliyor
            GMapRoute route = new GMapRoute(noktalar, "1");
            route.Stroke = new Pen(Color.Red, 3);
            routes.Routes.Add(route);

            noktalar = new List<PointLatLng>();
            //indirgenmiş kordinatlar noktalar listesine ekleniyor
            foreach (var koordinat in indirgenmisKordinatlar)
            {
                noktalar.Add((PointLatLng)koordinat);
            }
            //indirgenmiş noktalar arası mavi yollar çiziliyor 
            route = new GMapRoute(noktalar, "2");
            route.Stroke = new Pen(Color.Blue, 3);
            routes.Routes.Add(route);
        }

        //Dosyaki kordinatlara göre haritadaki yeşil noktaları ve sorgulanma işlemi gerçekleştirilmişse kırmızı noktaları oluşturan metot
        private void noktalariCiz()
        {
            foreach (var koordinat in kordinatlar)
            {
                //herbir nokta yeşil olarak haritaya veriyor
                var renk = GMarkerGoogleType.green;
                //eğer sorgulanmış ise sorgulanan herbir nokta kırmızı olarak haritaya veriliyor
                if (sorgulanmisKordinatlar.Where(x => x.Adi == koordinat.Adi).Count() > 0)
                {
                    renk = GMarkerGoogleType.red;
                }
               
                var marker = new GMarkerGoogle((PointLatLng)koordinat, renk);
                //Nokta adları daima görünsün kısmı
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = koordinat.Adi;
                routes.Markers.Add(marker);
            }
        }

        //İndirgeme servisini çağıran metot
        private void indirgemeServisiCagirma()
        {
            //Timer oluşturuluyor ve sunucu adresindeki servisi çağrıcak nesne oluşturuluyor
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var client = new RestClient("http://" + sunucuIp + ":" + sunucuPort + "/api/IndirgemeServisi");
            //servis çağrısı hazırlanıyor ve çağrıdaki bodynin json olduğu söyleniyor
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //bodye kordinatlar eklenip servis çağrısı yapılıyor
            request.AddJsonBody(kordinatlar);
            var response = client.Execute<Sonuc>(request);
            //gelen sonucta indirgenmiş kordinatlara aktarılıyor ve timer durdurluyor
            indirgenmisKordinatlar = response.Data.List;
            watch.Stop();
            //gecen sure milliseconds cinsinden hesaplanıp textboxmessage yazdırılıyor ve kordinatları çiziyor
            var sure = watch.Elapsed.TotalMilliseconds;
            textBoxMessage.Text = textBoxMessage.Text + Environment.NewLine + response.Data.Mesaj + " Toplam Sure : " + sure + "ms";
            kordinatCiz();
        }
        //sorgu servisini çağıran metot
        private void sorguServisiCagirma(TarananAlan tarananAlan)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            tarananAlan.Koordinatlar = kordinatlar;
            var client = new RestClient("http://" + sunucuIp + ":" + sunucuPort + "/api/SorgulamaServisi");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(tarananAlan);
            var response = client.Execute<Sonuc>(request);
            sorgulanmisKordinatlar = response.Data.List;
            watch.Stop();
            var sure = watch.Elapsed.TotalMilliseconds;

            textBoxMessage.Text = textBoxMessage.Text + Environment.NewLine + response.Data.Mesaj + " Toplam Sure : " + sure + "ms";
            kordinatCiz();
        }

        //Fareye tıklanıp tıklanmadığı ve nerede tıklandığı bilgisinin olduğu metot
        private void gMapControl_MouseDown(object sender, MouseEventArgs e)
        {
            isDown = true;
            initialX = e.X;
            initialY = e.Y;
        }

        //mause down olduysa tarama yaptıran metot
        private void gMapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDown == true)
            {
                this.Refresh();
                Pen drwaPen = new Pen(Color.Navy, 1);
                int width = e.X - initialX, height = e.Y - initialY;
                rect = new Rectangle(Math.Min(e.X, initialX),
                                Math.Min(e.Y, initialY),
                                Math.Abs(e.X - initialX),
                                Math.Abs(e.Y - initialY));

                formGraphics = gMapControl.CreateGraphics();
                formGraphics.DrawRectangle(drwaPen, rect);
            }
        }

        //fare kalktığı an taranan son alanı sorgulayan metot
        private void gMapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (formGraphics != null)
            {
                var tarananAlan = new TarananAlan();

                var x1 = Convert.ToInt32(rect.X);
                var y1 = Convert.ToInt32(rect.Y);
                var point1 = new GeoLife
                {
                    Latitude = gMapControl.FromLocalToLatLng(x1, y1).Lat,
                    Longitude = gMapControl.FromLocalToLatLng(x1, y1).Lng
                };
                tarananAlan.Baslangic = point1;

                var x4 = Convert.ToInt32(rect.X + rect.Width);
                var y4 = Convert.ToInt32(rect.Y + rect.Height);
                var point4 = new GeoLife
                {
                    Latitude = gMapControl.FromLocalToLatLng(x4, y4).Lat,
                    Longitude = gMapControl.FromLocalToLatLng(x4, y4).Lng
                };
                tarananAlan.Bitis = point4;

                sorguServisiCagirma(tarananAlan);
                isDown = false;
                formGraphics = null;
            }
        }

        //indirgeme labeli tıklandığında indirgeme servisini çağıran metot
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            indirgemeServisiCagirma();
        }
        //reset labeli tıklandığında reset işlemini gerçekleştiren metot
        private void toolStripLabel5_Click(object sender, EventArgs e)
        {
            sorgulanmisKordinatlar = new List<GeoLife>();
            indirgenmisKordinatlar = new List<GeoLife>();
            textBoxMessage.Text = "";
            kordinatCiz();
        }
    }
}
