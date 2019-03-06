using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using TrajectoryVerisiIsleme.WepApi.Model;

namespace TrajectoryVerisiIsleme.WepApi.Controllers
{

    public class IndirgemeServisiController : ApiController
    {
        // POST api/<controller>
        public Sonuc Post([FromBody]List<GeoLife> request)
        {
            //Timer oluşturuluyor
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var sonuc = new List<GeoLife>();
            sonuc.Add(request.First());
            //ilk elemana uzaklıklarına göre ağaç oluşturuluyor
            TreeNode tree = new TreeNode(request.First()).constructBalancedTree(request.Where(x => x.Adi != request.First().Adi));
            //ağaçta son elemanı bulana kadar gezdiği her elemanı listeye ekliyor
            foreach (var leaf in tree)
            {
                sonuc.Add(leaf.Value);
                if (leaf.Value == request.Last())
                {
                    break;
                }
            }
            //timer durdurluyor
            watch.Stop();
            //gecen sure, oran hesaplanıp mesaj oluşturuluyor ve sonuc döndürülüyor
            var sure = watch.Elapsed.TotalMilliseconds;
            var oran = (1.0 - (double)sonuc.Count / (double)request.Count) * 100;
            var mesaj = "IndirgemeServisi :" + " Oran : %" + oran + " İslem Sure : " + sure + "ms";

            return new Sonuc
            {
                Mesaj = mesaj,
                List = sonuc
            };
        }
    }
}