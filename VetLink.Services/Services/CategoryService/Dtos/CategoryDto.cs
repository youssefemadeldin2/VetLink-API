using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VetLink.Services.Services.CategoryService.Dtos
{
    public class CategoryDto
    {
        public int Id{ get; set; }
        public string Name { get; set; }
		public int? ParentId { get; set; }
	}
}
