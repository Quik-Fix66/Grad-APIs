using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Models.Ecom.Rating
{
	public class Rating
	{
		[Key]
		public Guid RatingId { get; set; }
		public double OverallRating { get; set; }
	}
}

