using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTest.WebApp.Controllers;
using UnitTest.WebApp.Helpers;
using UnitTest.WebApp.Model;
using UnitTest.WebApp.Repository;
using Xunit;


namespace UnitTest.WebApp.Test
{
    public class ProductsApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;
        private readonly Helper _helper;
        private List<Product> products;

        public ProductsApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsApiController(_mockRepo.Object);
            _helper = new Helper();

            products = new List<Product>
            {
                new Product{ Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" },
                new Product{ Id = 2, Name = "Defter", Price = 200, Stock = 500, Color = "Mavi" }
            };
        }

        [Fact]
        public async void GetProduct_ActionExecutes_ReturnOkResultwithProduct()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products);

            var result = await _controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal(2, returnProduct.ToList().Count);
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(default(Product));

            var result = await _controller.GetProduct(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdIsValid_ReturnOkResult(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);

            var okObjectResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsType<Product>(okObjectResult.Value);

            Assert.Equal(product, returnProduct);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = products.First(x => x.Id == productId);

            var result = _controller.PutProduct(2, product);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.Update(product));

            var result = _controller.PutProduct(productId, product);

            _mockRepo.Verify(x => x.Update(product), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostProduct_ActionExecutes_ReturnCreatedAtAction()
        {
            var product = products.First();

            _mockRepo.Setup(x => x.Create(product)).Returns(Task.CompletedTask);

            var result = await _controller.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mockRepo.Verify(x => x.Create(product), Times.Once);

            Assert.Equal(nameof(ProductsApiController.GetProduct), createdAtActionResult.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId)
        {
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(default(Product));

            var resultNotFound = await _controller.DeleteProduct(productId);

            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecute_ReturnNoContent(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            _mockRepo.Setup(x => x.Delete(product));

            var resultNoContentresult = await _controller.DeleteProduct(productId);

            _mockRepo.Verify(x => x.Delete(product), Times.Once);

            Assert.IsType<NoContentResult>(resultNoContentresult.Result);
        }

        [Theory]
        [InlineData(4,5,9)]
        public void Add_SampleValue_ReturnTotal(int a,int b,int total)
        {
            var result = _helper.add(a,b);

            Assert.Equal(result,total);
        }
    }
}