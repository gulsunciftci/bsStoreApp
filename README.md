Merhaba bu repoda Asp .Net Core 6 web api projesi oluşturacağım ve aşamalarını sizlerle paylaşacağım 👩🏻‍💻


# EF CORE KULLANIMI:
1) Varlık tanımı yapacağım(Entities/Models)
2) Repository Context
3) Connection String
4) Migrations
5) Type Configuration(Tipleri yapılandırma)
6) Inversion of Control
7) API Testi
* Entity Framework (EF) Core, popüler Entity Framework veri erişim teknolojisinin basit, genişletilebilir, açık kaynaklı ve platformlar arası bir sürümüdür.
* EF Core, nesne ilişkisel bir eşleyici (O/RM) olarak görev yapabilir ve bu şunları getirir:
1) .NET geliştiricilerinin .NET nesnelerini kullanarak bir veritabanıyla çalışmasını sağlar.
2) Genellikle yazılması gereken veri erişim kodunun çoğuna olan ihtiyacı ortadan kaldırır.
* İki temel yaklaşım vardır burda
1) Code first
2) Database first
Ben Code First yaklaşımı ile ilerledim

* bsStoreApp solution içerisine Projeyi oluşturdum:

# 📁 [WebApi](https://github.com/gulsunciftci/bsStoreApp/tree/main/WebApi)

* İlk olarak MVC patternini dikkate alıyorum ancak view ifadem olmayacak

1) Models dosyası oluşturdum:
* İçerisine Book isminde bir class oluşturup propertylerimi ekledim
* Çalıştırdığımda operasyonu olmayan bir swaggerla karşılaşırım çünkü controller yok

```C#
namespace WebApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }

    }
}

```
2) RepositoriesContext dosyası oluşturdum
* Bir class ekledim ve RepositoryContext ismini verdim
* DbContext'ten kalıtım aldım. Bunun için Microsoft.EntityFrameworkCore paketini indirdim. Artık elimde bir contexim var. Context veriye erişim konusunda yardımcı oluyor.
* DbSet tanımı yaptım. 
* Veri tabanının bağlantı dizesine sahip olması gerekiyor bunu bir constructur ile ekliyorum.
* Yapılandırma yapmam gerekiyor bunun için appsettings.json dosyasına aşağıdaki kodu ekliyorum.
```C#
"ConnectionStrings": {
    "sqlConnection": "Server=(localdb)\\ProjectModels;database=bsStoreApp; integrated security=true; Trusted_Connection=True;MultipleActiveResultSets=true"
  }
```
* Program.cste yapılandırma yapmam gerekiyor bunun için Microsoft.EntityFrameworkCore.SqlServer paketini yükledim. Aşağıdaki kodu ekledim.

```C#
builder.Services.AddDbContext<RepositoryContext>(options =>
                   options.UseSqlServer(
                       builder.Configuration.GetConnectionString("sqlConnection"))
                   );
```

```C#
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Repositories.Config;

namespace WebApi.Repositories
{
    public class RepositoryContext:DbContext
    {
       public RepositoryContext (DbContextOptions options):base(options)
       {
           //Bağlantı dizesi
       }
       public DbSet<Book> Books { get; set; }
       
    }
}

```
* Migration işlemi yapmam gerekiyor bunun için Ef core tools,design paketlerini yükledim. Ardından Migration işlemini yaptım. Database'e update ettim. Tablomuz artık oluştu.
* Repository klasörünün altına Config dosyası oluşturdum ve içerisine BookConfig class'ı oluşturdum. 3 Çekirdek veri ekledim.

```C#
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Models;

namespace WebApi.Repositories.Config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasData( //params
                new Book { Id = 1, Title = "Karagöz ve Hacivat", Price = 75 },
                new Book { Id = 2, Title = "Mesnevi", Price = 175 },
                new Book { Id = 3, Title = "Devlet", Price = 375 }
            );
        }
    }
}
```
* Migration alıp çekirdek verileri database'e update edebilmek için RepositoryContext içerisine aşağıdaki kodu ekledim.
```C#
 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
            modelBuilder.ApplyConfiguration(new BookConfig());
 }
```

* RepositoryContext:
```C#
 using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Repositories.Config;

namespace WebApi.Repositories
{
    public class RepositoryContext:DbContext
    {
       public RepositoryContext (DbContextOptions options):base(options)
       {

       }
       public DbSet<Book> Books { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());
        }
    }
}
```
* Bu işlemi yaptıktan sonra migration ve database update işlemlerini yaptım.
* Veri tabanı tanımını gerçekleştirdim veri tabanını kullanabilmem için inversion of control kullanmak gerekiyor bunun için controllera ihtiyacım var bu sebeple BooksController ekleyerek Http isteklerimi ekledim.
```C#
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly RepositoryContext _repositoryContext;

        public BooksController(RepositoryContext repositoryContext) //Dependency injection
        { //Resolve
            _repositoryContext = repositoryContext;
        }

        [HttpGet] 
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _repositoryContext.Books.ToList();
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
                var book = _repositoryContext.
              Books.
              Where(b => b.Id.Equals(id))
              .SingleOrDefault();
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
                _repositoryContext.Books.Add(book);
                _repositoryContext.SaveChanges();
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
                var entity = _repositoryContext.Books.Where(b => b.Id.Equals(id))
                .SingleOrDefault();
                if (entity is null)
                {
                    return NotFound(); //404
                }
                //check id
                if (id != book.Id)
                {
                    return BadRequest(); //400
                }

                entity.Title = book.Title;
                entity.Price = book.Price;
                _repositoryContext.SaveChanges();
                return Ok(book);
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
                var entity = _repositoryContext
                 .Books
                 .Where(b => b.Id.Equals(id)).SingleOrDefault();
                if (entity is null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        message = $"Book with id:{id} could not found"
                    }); //404
                }

                _repositoryContext.Books.Remove(entity);
                _repositoryContext.SaveChanges();
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
                var entity = _repositoryContext.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();
                if (entity is null)
                {
                    return NotFound(); //404
                }

                bookPatch.ApplyTo(entity);
                _repositoryContext.SaveChanges();
                return NoContent(); //204
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

```

# KATMANLI MİMARİ KULLANIMI

1) 📁 [Entities](https://github.com/gulsunciftci/bsStoreApp/tree/main/Entities)

* Yeni bir class Library ekledim ve ismini Entities koydum.
* İçine Model klasörü ekledim.
* Book varlığımı buraya taşıdım.

```C#
 public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }

    }
```

2) 📁 [Repositories](https://github.com/gulsunciftci/bsStoreApp/tree/main/Repositories)

* Bir sınıf kütüphanesi oluşturdum ve adını Repositories verdim.
* interface yapılarını Contractlar olarak değerlendirdim ve bunları eklemek için Contracts isminde bir klasör oluşturdum.
* IRepositoryBase interfaceini ekledim. Bir imza oluşturdum.

```C#
  public interface IRepositoryBase<T>
    {
        //Sorgulanabilir ifadeler
        //değişiklikleri izleyip izlememek için bunu bir parametreye bağlıyoruz trackChanges bunu ifade ediyor
        //CRUD
        IQueryable<T> FindAll(bool trackChanges);

        //T:Generic
        //Func:Delege
        IQueryable<T> FindByCondition(Expression<Func<T,bool>> expression,bool trackChanges);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    
    }
```

* EFCore klasörü oluşturdum.
* İçerisine RepositoryContext ekledim.
```C#
  public class RepositoryContext:DbContext
    {
       public RepositoryContext (DbContextOptions options):base(options)
       {

       }
       public DbSet<Book> Books { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());
        }
    }
```

* Config dosyasını EFCore içine taşıdım.
* IRepositoryBase'i implemente edecek classa ihtiyaç var bu sebeple EFCore içerisine RepositoryBase class'ını oluşturdum.

* NOT: Contractlar hangi metotların implemente edileceğini söyler detaylarla ilgilenmez.
* Api projesine yeni klasör ekliyorum ismini Extensions koydum.içerisine ServiceExtensions classını ekledim. Uzantı metotlarını içerir. Servislere ait tanımları içerir.
```C#
  public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)=>
            services.AddDbContext<RepositoryContext>(options =>
                           options.UseSqlServer(
                               configuration.GetConnectionString("sqlConnection")));

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();


        public static void ConfigureServiceManager(this IServiceCollection services)=>
            services.AddScoped<IServiceManager, ServiceManager>();
    }
```
* Congfiguration ifadesini program.cs ten silip daha kısa halini ekledim.
```C#
builder.Services.ConfigureSqlContext(builder.Configuration);
```

3) 📁 [Services](https://github.com/gulsunciftci/bsStoreApp/tree/main/Services)

* Servis katmanını ekledim.(class library)

4) 📁 [Presentation](https://github.com/gulsunciftci/bsStoreApp/tree/main/Presentation)
* Sunum katmanını ekledim.(class library)
* Controller'ı Apidan buraya taşıdım.
```C#

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

```

# AUTOMAPPER KULLANIMI:

* İlk olarak aşağıdaki paketi indirdim
```C#
automapper.extensions.microsoft.dependencyinjection
```
* Daha sonra program.cs'e şu kod satırını ekledim
```C#
builder.Services.AddAutoMapper(typeof(Program));
```
### DTO NEDİR?
* AutoMapper, projemizde Entity nesnelerini database’den çektiğimiz haliyle değil, bu nesneleri istediğimiz (UI’da bizim için gerekli olacak) formata çevirmemizi sağlayan basit bir kütüphanedir. DTO (Data Transfer Object) ise AutoMapper’ın dönüştürmesini istediğimiz format modelidir.
![Alt text](image.png)
