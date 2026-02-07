using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetLink.Data.Entities;

namespace VetLink.Repository.Specifications.ImageSpecifications
{
    public class ImageWithSpecs:BaseSpecification<Image>
    {
        public ImageWithSpecs(int productId)
            :base(i=>i.ProductId == productId )
        {
            
        }
    }
}
