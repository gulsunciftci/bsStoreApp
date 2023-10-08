﻿
using Entities.DataTransferObject;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System.Text.Json;

namespace Presentation.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
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
        public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParameters bookParameters)
        {
            
            var pagedResult = await _manager.BookService.GetAllBooksAsync(bookParameters,false);
                

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

            return Ok(pagedResult.books);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name="id")]int id)
        {
            
                //throw new Exception("!!!!!");
                var book = await _manager.
                BookService.GetOneBookByIdAsync(id,false);

                return Ok(book);
            
        }
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost] //kitap eklemek için 
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto) //veri tabanı ıd yi kendisi veriyor
        {
           
                var book= await _manager.BookService.CreateOneBookAsync(bookDto);     
                return StatusCode(201, book);//CreatedAtRoute()
          
        }
       
        [ServiceFilter(typeof(ValidationFilterAttribute),Order =1)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody]BookDtoForUpdate bookDto)
        {
           
               await _manager.BookService.UpdateOneBookAsync(id, bookDto, false);    
              return NoContent();//204
          
        }
        
     
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {


            await _manager.BookService.DeleteOneBook(id, false);

                return NoContent();
           
        }

        
        [HttpPatch("{id:int}")] //Puttan farkı putta nesneyi bir bütün olarak güncelliyoruz burada ise kısmi güncelleme yapabiliyoruz.
        // Normalde bir array içinde tanımlanır
        public async Task<IActionResult>  PartiallyUpdateOneBook([FromRoute(Name = "id")] int id, 
            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
                if(bookPatch is null)
                {
                     return BadRequest(); //400
                }

                var result = await _manager.BookService.GetOneBookForPatchAsync(id,false);
                
                
                bookPatch.ApplyTo(result.bookDtoForUpdate,ModelState);

                TryValidateModel(result.bookDtoForUpdate);
                if (!ModelState.IsValid)
                {
                    return UnprocessableEntity(ModelState);
                }

            await _manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate,result.book);
                return NoContent(); //204
           

        }
    }
}
