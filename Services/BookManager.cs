﻿using Entities.Exceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly IRepositoryManager _manager;
        private readonly ILoggerService _loggerService;
      
        public BookManager(IRepositoryManager manager,ILoggerService loggerService) //DI
        {
            _manager = manager;
            _loggerService = loggerService;
        }

        public Book CreateOneBook(Book book)
        {
            if(book is null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            _manager.Book.CreateOneBook(book);
            _manager.Save();
            return book;
        }

        public void DeleteOneBook(int id, bool trackChanges)
        {
            var entity = _manager.Book.GetOneBookById(id, trackChanges);
            if(entity is null)
            {
                
                    throw new BookNotFoundException(id);
                
            }

            _manager.Book.DeleteOneBook(entity);
            _manager.Save();
        }


        public IQueryable<Book> GetAllBooks(bool trackChanges)
        {

            return _manager.Book.GetAllBooks(trackChanges);
        }

        public Book GetOneBookById(int id, bool trackChanges)
        {
            var book= _manager.Book.GetOneBookById(id, trackChanges);
            if (book is null)
            {
                throw new BookNotFoundException(id);
            }
            return book;
        }

        public void UpdateOneBook(int id, Book book, bool trackChanges)
        {
            //check entity
            var entity = _manager.Book.GetOneBookById(id,trackChanges);
            if (entity is null)
            {
               
                    throw new BookNotFoundException(id);
                

            }
            //check params
            if(book is null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            entity.Title = book.Title;
            entity.Price = book.Price;

            _manager.Book.Update(entity);
            _manager.Save();

        }
    }
}
