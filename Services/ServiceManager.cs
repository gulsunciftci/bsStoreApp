﻿using AutoMapper;
using Entities.DataTransferObject;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
	public class ServiceManager : IServiceManager
	{
		private readonly Lazy<IBookService> _bookService;
		private readonly Lazy<IAuthenticationService> _authencationService;

		public ServiceManager(IRepositoryManager repositoryManager,
			ILoggerService loggerService,
			IMapper mapper,
			IConfiguration configuration,
			UserManager<User> userManager,
			IBookLinks bookLinks)
		{
			_bookService = new Lazy<IBookService>(()=>
			new BookManager(repositoryManager,loggerService,mapper,bookLinks));
			_authencationService = new Lazy<IAuthenticationService>(() =>
			new AuthenticationManager(loggerService, mapper,userManager,configuration));
		}

		public IBookService BookService => _bookService.Value;

		public IAuthenticationService AuthenticationService => _authencationService.Value;

		
	}
}
