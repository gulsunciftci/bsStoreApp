using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager) //Dependency injection
        { //Resolve
            _manager = manager;
        }

        [HttpGet] 
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _manager.BookService.GetAllBooks(false);
                return Ok(books);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name="id")]int id)
        {
            try
            {
                var book = _manager.
              BookService.GetOneBookById(id,false);
              
                if (book is null)
                {
                    return NotFound();
                }

                return Ok(book);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }




        [HttpPost] //kitap eklemek için 
        public IActionResult CreateOneBook([FromBody] Book book) //veri tabanı ıd yi kendisi veriyor
        {
            try
            {
                if (book is null)
                {
                    return BadRequest();
                }
                _manager.BookService.CreateOneBook(book);
                
                return StatusCode(201, book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, Book book)
        {
            try
            {
               if(book is null)
                {
                    return BadRequest();//400
                }

                _manager.BookService.UpdateOneBook(id, book, false);
               
                return NoContent();//204
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
        
     
        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            try
            {
               
                _manager.BookService.DeleteOneBook(id, false);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("{id:int}")] //Puttan farkı putta nesneyi bir bütün olarak güncelliyoruz burada ise kısmi güncelleme yapabiliyoruz.
        // Normalde bir array içinde tanımlanır
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            try
            {
                //check entity
                var entity = _manager.BookService.GetOneBookById(id, true);
                if (entity is null)
                {
                    return NotFound(); //404
                }

                bookPatch.ApplyTo(entity);
                _manager.BookService.UpdateOneBook(id,entity,true);
                return NoContent(); //204
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
