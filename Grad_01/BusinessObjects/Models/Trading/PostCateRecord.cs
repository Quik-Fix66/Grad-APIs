//using System;
//using System.ComponentModel.DataAnnotations;
//using BusinessObjects.Models.E_com.Trading;

//namespace BusinessObjects.Models.Trading
//{
//	public class PostCateRecord
//	{
//		[Key]
//		public Guid Id { get; set; }
//		public Guid CateId { get; set; }
//		public Guid PostId { get; set; }

//		[ForeignKey("CateI")]
//		Category Cate { get; set; } = null!;
//		private Post Post { get; set; } = null!;
//	}
//}

