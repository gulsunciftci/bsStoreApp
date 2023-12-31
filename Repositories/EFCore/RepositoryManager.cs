﻿using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class RepositoryManager : IRepositoryManager //implemente etti
    {

        private readonly IBookRepository _bookRepository; //dbcontext
		private readonly ICategoryRepository _categoryRepository;
		//injection
		private readonly RepositoryContext _context;

		public RepositoryManager(IBookRepository bookRepository, ICategoryRepository categoryRepository, RepositoryContext context)
		{
			_bookRepository = bookRepository;
			_categoryRepository = categoryRepository;
			_context = context;
		}
		///<summary>
		///Lazy Loading:
		///İsteğe bağlı olarak modüllerin yüklenmesi işlemidir.(Js, CSS, video, doküman, resim vb.)
		/// Diyelim ki 100 sayfalık teknik bir doküman okuyorsunuz. 
		/// O dokümanda size yalnızca son 15 sayfadaki bilgi lazım.
		/// Fakat siz bütün dokümanının işinize yaramayacağını, 
		/// içindekiler sayfasını okuduğunuz halde sayfaları tek tek okuyorsunuz. 
		/// Bu size iki şey kaybettirir.
		/// 1.Vakit
		/// 2.Performans
		/// Gereğinden fazla belki de işinize yaramayacak alanları okuduğunuz için vaktinizi, 
		/// ihtiyacınızdan fazla bilgiyi okuduğunuzda da performansınızı kaybetmiş olacaksınız.
		/// İşte Lazy Loading sizin yalnızca son 15 sayfayı okumanızı sağlayacak ortamı sağlıyor. 
		/// Ve ihtiyacınız dahilinde, yalnızca siz istediğinizde öteki sayfaları okuyabiliyorsunuz.
		/// </summary>
		public IBookRepository Book => _bookRepository;

		public ICategoryRepository Category => _categoryRepository;

		public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
