using ElasticSearch.Example.Repository;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System.Resources;

namespace ElasticSearch.Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepository;

        public ProductController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        [HttpPost("Insert")]
        public async Task InsertAsync()
        {
            await productRepository.InsertAsync(new Product()
            {
                Id = 1,
                Name = "´ó°×²Ë"
            });

            var list = new List<Product>()
            {
                new Product()
                {
                    Id=2,
                    Name="ÍÁ¶¹"
                },
                new Product()
                {
                    Id=3,
                    Name="ÇÑ×Ó"
                },
                new Product()
                {
                    Id=2,
                    Name="¶¹½Ç"
                },
            };
            await productRepository.InsertManyAsync(list);
        }

        [HttpGet("Get")]
        public async Task<Product> Get(long id)
        {
            return await productRepository.GetAsync(id);
        }


        [HttpGet("GetList")]
        public async Task<IEnumerable<Product>> GetListAsync([FromQuery]ProductSearchModel model)
        {
            var must = new List<Func<QueryContainerDescriptor<Product>, QueryContainer>>();
            if (!string.IsNullOrEmpty(model.Name))
                must.Add(x => x.Match(m => m.Field(f => f.Name).Query(model.Name)));
            if (!string.IsNullOrEmpty(model.Description))
                must.Add(x => x.Match(m => m.Field(f => f.Description).Query(model.Description)));


            var descriptor = new SearchDescriptor<Product>().From((model.PageIndex - 1) * model.PageSize)
                                                            .Size(model.PageSize)
                                                            .Query(q => q.Bool(b => b.Must(must)))
                                                            .TrackTotalHits(true);

            var (list, count) = await productRepository.SearchAsync(descriptor);
            return list;
        }
    }

    public class ProductSearchModel
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public string? Name { get; set; }

        public string? Description { get; set; }
    }

}