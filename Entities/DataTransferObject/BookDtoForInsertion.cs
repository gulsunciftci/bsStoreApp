using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.DataTransferObject
{
    public record BookDtoForInsertion : BookDtoForManipulation
    {
		//[Required(ErrorMessage = "CategoryId is required.")]
		//public int CategoryId { get; init; }
		[Required(ErrorMessage = "CategoryId is required.")]
		public int CategoryId { get; init; }
		[JsonIgnore]
        public int Id { get; set; }
    }
}
