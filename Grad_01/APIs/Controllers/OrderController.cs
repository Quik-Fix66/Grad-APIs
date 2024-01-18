using System;
using APIs.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		public IBookRepository _bookRepo;

		public OrderController(IBookRepository bookRepo)
		{
			_bookRepo = bookRepo;
		}

		//
		//Create order (CartToOrderDTO)
		//
		//[HttpPost]
		//[Route("create-order")]
		//public IActionResult CreateOrder([FromBody] CartToOrderDTO data)
		//{

		//}
		
	}
}

