using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrajectoryVerisiIsleme.WinForm
{
    //Taran alan Geolife türünde değişkenlerinin olduğu sınıf
    public class TarananAlan
    {
        public GeoLife Baslangic { get; set; }

        public GeoLife Bitis { get; set; }

        public List<GeoLife> Koordinatlar { get; set; }
    }
    //Geolife değişkenlerinin olduğu sınıf
    public class GeoLife
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int Data0 { get; set; }

        public double Altitude { get; set; }

        public double DateNumber { get; set; }

        public string DateString { get; set; }

        public string TimeString { get; set; }

        public string Adi { get; set; }

        //Dosya satırlarını geolife nesnesine çeviren metot(geolife casting işlemi)
        public static explicit operator GeoLife(string text)
        {
            var array = text.Split(',');
            GeoLife data = new GeoLife();
            data.Latitude = Convert.ToDouble(array[0]);
            data.Longitude = Convert.ToDouble(array[1]);
            data.Data0 = Convert.ToInt32(array[2]);
            data.Altitude = Convert.ToDouble(array[3]);
            data.DateNumber = Convert.ToDouble(array[4]);
            data.DateString = Convert.ToString(array[5]);
            data.TimeString = Convert.ToString(array[6]);

            return data;
        }
        //Geolife nesnesini PointLatLng nesnesine çeviren metot(geolifefı pointlatlng casting işlemi)
        public static explicit operator PointLatLng(GeoLife point)
        {
            return new PointLatLng(point.Latitude, point.Longitude);
        }
    }
    //Servislerden dönen listeler ve mesajlar için oluşturulan sınıf
    public class Sonuc
    {
        public string Mesaj { get; set; }

        public List<GeoLife> List { get; set; }
    }
}
