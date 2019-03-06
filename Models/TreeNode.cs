using System.Collections;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using TrajectoryVerisiIsleme.WepApi.Model;

namespace TrajectoryVerisiIsleme.WepApi.Model
{
    //forech içinde treenin elemanlarında gezebilmek için IEnumerator interface'ini implement ettik
    //BST indeksleme işleminin gerçekleştiği sınıf
    public class TreeNode : IEnumerator<TreeNode>
    {
        public bool Visited { get; set; }

        public GeoLife Value { get; private set; }

        public TreeNode Parent { get; set; }

        public TreeNode Left { get; set; }

        public TreeNode Right { get; set; }

        private TreeNode current;

        public TreeNode Current => current;

        object IEnumerator.Current => current;

        //const
        public TreeNode(GeoLife value)
        {
            Value = value;
        }
        //ağacı dengeli şekilde oluşturan metot
        private TreeNode constructBalancedTree(List<GeoLife> values, int min, int max)
        {
            //ağaçta yerleştirme işlemi bittiyse null dönmesi işlemi
            if (min == max)
                return null;

            //orta nokta hesaplanıyor
            int median = min + (max - min) / 2;

            //yeni bir node oluşuturup sol ve sağ nodelarını yeniden hesaplıyor
            var node = new TreeNode(Value = values[median])
            {
                Left = constructBalancedTree(values, min, median),
                Right = constructBalancedTree(values, median + 1, max)
            };
            //mevcut nodeun parentı var ise sol nodenun parentını kendi olarak set ediyor
            if(node.Left != null)
            {
                node.Left.Parent = node;
            }
            //mevcut nodeun parentı var ise sol nodenun parentını kendi olarak set ediyor
            if (node.Right != null)
            {
                node.Right.Parent = node;
            }
            //yeni oluşturulan node döndürülüyor
            return node;
        }

        //kendisine göre gelen diğer elemanları sıralayıp ağacı dengeli şekilde oluşturmak için private metoduna gönderiyor
        public TreeNode constructBalancedTree(IEnumerable<GeoLife> values)
        {
            var first = new GeoCoordinate(Value.Latitude, Value.Longitude);
            return constructBalancedTree(values.OrderBy(x => first.GetDistanceTo(new GeoCoordinate(x.Latitude, x.Longitude))).ToList(), 0, values.Count());
        }
        
        public void Dispose()
        {
        }
        //Bir sonraki node var mı kontrolü yapan var ise currentta atayan metot
        public bool MoveNext()
        {
            //forechte gezinirken 

            //current null ise current kendisi yapar
            if (current == null)
            {
                current = this;
            }
            //currenttın sol elemanı var ise current o olur
            else if (current.Left != null)
            {
                current = current.Left;
            }
            //currenttın sağ elemanı var ise current o olur
            else if (current.Right != null)
            {
                current = current.Right;
            }
            //currenttın sol ve sağ elemanı yok ise
            else
            {
                //Bir üst node'un sağ nodu gezilmemişse sağ node current olur
                //Eğer sağ node gezilmişse gezilmemiş sağ node bulmak için parentlara yukarı gider
                
                var node = current;
                do
                {
                    //Eğer mevcut node'un parentı null ise ana node'a çıkılmıştır
                    if (node.Parent == null)
                    {
                        return false;
                    }
                    node = node.Parent;
                } while (node.Right == null || node.Right.Visited);
                current = node.Right;
            }
            //current bulunduğu için gezilmiştir bilgisini true yapar
            current.Visited = true;
            return true;
        }

        //current noktasını null yapar
        public void Reset()
        {
            current = null;
        }
        //forechin çalışması için gerekli metot
        public virtual TreeNode GetEnumerator()
        {
            return this;
        }
    }
}