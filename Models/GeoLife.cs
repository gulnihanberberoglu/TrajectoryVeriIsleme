using System;
using System.Collections.Generic;

namespace TrajectoryVerisiIsleme.WepApi.Model
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

        public string Adi { get; set; }

        public double Altitude { get; set; }

        public double DateNumber { get; set; }

        public string DateString { get; set; }

        public string TimeString { get; set; }
    }
    //Servislerden dönen listeler ve mesajlar için oluşturulan sınıf
    public class Sonuc
    {
        public string Mesaj { get; set; }

        public List<GeoLife> List { get; set; }
    }
}
