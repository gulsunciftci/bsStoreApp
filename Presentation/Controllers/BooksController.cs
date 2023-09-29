
using Entities.DataTransferObject;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase //Kalıtım
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager) //Dependency injection
        { //Resolve
            _manager = manager;
        }

        [HttpGet] 
        public IActionResult GetAllBooks()
        {
            
                var books = _manager.BookService.GetAllBooks(false);
                return Ok(books);
          

        }


        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name="id")]int id)
        {
            
                //throw new Exception("!!!!!");
                var book = _manager.
                BookService.GetOneBookById(id,false);

                return Ok(book);
            
        }

        [HttpPost] //kitap eklemek için 
        public IActionResult CreateOneBook([FromBody] Book book) //veri tabanı ıd yi kendisi veriyor
        {
           
                if (book is null)
                {
                    return BadRequest();
                }
                _manager.BookService.CreateOneBook(book);
                
                return StatusCode(201, book);
          
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody]BookDtoForUpdate bookDto)
        {
           
               if(bookDto is null)
                {
                    return BadRequest();//400
                }

                _manager.BookService.UpdateOneBook(id, bookDto, false);
               
                return NoContent();//204
          
        }
        
     
        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            
               
                _manager.BookService.DeleteOneBook(id, false);

                return NoContent();
           
        }


        [HttpPatch("{id:int}")] //Puttan farkı putta nesneyi bir bütün olarak güncelliyoruz burada ise kısmi güncelleme yapabiliyoruz.
        // Normalde bir array içinde tanımlanır
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
           
                //check entity
                var entity = _manager.BookService.GetOneBookById(id, true);
               
                bookPatch.ApplyTo(entity);
                _manager.BookService.UpdateOneBook(id,new BookDtoForUpdate(entity.Id,entity.Title,entity.Price),
                true);
                return NoContent(); //204
           

        }
    }
}
