
using Entities.DataTransferObject;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System.Text.Json;

namespace Presentation.Controllers
{
	//[ApiVersion("1.0")]
	[ApiExplorerSettings(GroupName = "v1")]
	[ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[ResponseCache(CacheProfileName ="5mins")]
    //[HttpCacheExpiration(CacheLocation =CacheLocation.Private,MaxAge =60)]
    public class BooksController : ControllerBase //Kalıtım
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager) //Dependency injection
        { //Resolve
            _manager = manager;
        }

        //ENDPOİNTLER

        [Authorize] //Koruma
        [HttpGet]
        //[HttpHead]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        //[ResponseCache(Duration =60)] //60 saniye boyunca önbelleğe alınabilir
        public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParameters bookParameters)
        {
            var linkParameters = new LinkParameters()
            {
                BookParameters = bookParameters,
                HttpContext = HttpContext
            };
            var result = await _manager.BookService.
                GetAllBooksAsync(linkParameters,false);
                

            Response.Headers.Add("X-Pagination", 
                JsonSerializer.Serialize(result.metaData));

            return result.linkResponse.HasLinks ?
                Ok(result.linkResponse.LinkedEntities) :
                Ok(result.linkResponse.ShapedEntities);

		}

		[Authorize]
		[HttpGet("details")]
		public async Task<IActionResult> GetAllBooksWithDetailsAsync()
		{
			return Ok(await _manager
				.BookService
				.GetAllBooksWithDetailsAsync(false));
		}




		[Authorize] //Koruma
		[HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name="id")]int id)
        {
            
                //throw new Exception("!!!!!");
                var book = await _manager.
                BookService.GetOneBookByIdAsync(id,false);

                return Ok(book);
            
        }

		[Authorize(Roles = "Admin,Editor")] //Koruma
		[ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost] //kitap eklemek için 
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto) //veri tabanı ıd yi kendisi veriyor
        {
           
                var book= await _manager.BookService.CreateOneBookAsync(bookDto);     
                return StatusCode(201, book);//CreatedAtRoute()
          
        }

		[Authorize(Roles = "Admin,Editor")]
		[ServiceFilter(typeof(ValidationFilterAttribute),Order =1)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody]BookDtoForUpdate bookDto)
        {

			await _manager.BookService.UpdateOneBookAsync(id, bookDto, false);
			return NoContent(); // 204

		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {


            await _manager.BookService.DeleteOneBook(id, false);

                return NoContent();
           
        }

		[Authorize(Roles = "Admin,Editor")]
		[HttpPatch("{id:int}")] //Puttan farkı putta nesneyi bir bütün olarak güncelliyoruz burada ise kısmi güncelleme yapabiliyoruz.
        // Normalde bir array içinde tanımlanır
        public async Task<IActionResult>  PartiallyUpdateOneBook([FromRoute(Name = "id")] int id, 
            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
			if (bookPatch is null)
				return BadRequest(); // 400

			var result = await _manager.BookService.GetOneBookForPatchAsync(id, false);

			bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);

			TryValidateModel(result.bookDtoForUpdate);

			if (!ModelState.IsValid)
				return UnprocessableEntity(ModelState);

			await _manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);

			return NoContent(); // 204


		}


		[Authorize]
		[HttpOptions]
		public IActionResult GetBooksOptions()
		{
			Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS");
			return Ok();
		}

	}
}
