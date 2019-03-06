using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TrajectoryVerisiIsleme.WepApi.Model;

namespace TrajectoryVerisiIsleme.WepApi.Controllers
{
    public class SorgulamaServisiController : ApiController
    {
        // POST api/<controller>
        public Sonuc Post([FromBody]TarananAlan request)
        {
            //Timer oluşturuluyor
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //ilk eleman dahil ilk elemana uzaklıklarına göre ağaç oluşturuluyor
            TreeNode tree = new TreeNode(request.Koordinatlar.First())
                .constructBalancedTree(request.Koordinatlar);
            var sonuc = new List<GeoLife>();
            //ağaçtaki tüm elemanlar gezilir taranan alan içinde bulunan her bir eleman listeye eklenir
            foreach (var leaf in tree)
            {
                var koordinat = leaf.Value;
                if (request.Baslangic.Latitude >= koordinat.Latitude &&
                    request.Baslangic.Longitude <= koordinat.Longitude &&
                    request.Bitis.Latitude <= koordinat.Latitude &&
                    request.Bitis.Longitude >= koordinat.Longitude)
                {
                    sonuc.Add(koordinat);
                }
            }
            //timer durdurluyor
            watch.Stop();
            //gecen sure, oran hesaplanıp mesaj oluşturuluyor ve sonuc döndürülüyor
            var sure = watch.Elapsed.TotalMilliseconds;
            var oran = ((double)sonuc.Count / (double)request.Koordinatlar.Count) * 100;
            var mesaj = "SorgulamaServisi :" + " Oran : %" + oran + " İslem Sure : " + sure + "ms";

            return new Sonuc
            {
                Mesaj = mesaj,
                List = sonuc
            };
        }
    }
}