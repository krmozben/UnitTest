using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTest.WebApp.Controllers;
using UnitTest.WebApp.Model;
using UnitTest.WebApp.Repository;
using Xunit;

namespace UnitTest.WebApp.Test
{
    public class ProductsControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> products;

        public ProductsControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);

            products = new List<Product>
            {
                new Product{ Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" },
                new Product{ Id = 2, Name = "Defter", Price = 200, Stock = 500, Color = "Mavi" }
            };
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(ProductsController.Index), redirect.ActionName);
        }

        [Fact]
        public async void Details_IdIsInvalid_ReturnNotFound()
        {
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(default(Product));

            var result = await _controller.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_IdIsValid_ReturnProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal<Product>(resultProduct, product);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gerekli");

            var result = await _controller.Create(products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_IsValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(ProductsController.Index), redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_IsValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;

            _mockRepo.Setup(x => x.Create(It.IsAny<Product>())).Callback<Product>(p => { newProduct = p; });

            var result = await _controller.Create(products.First());

            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);

            Assert.Equal(newProduct, products.First());
        }

        [Fact]
        public async void CreatePOST_InValidModelState_NeverCreateMethodExecute()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gerekli");

            var result = await _controller.Create(products.First());

            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(ProductsController.Index), redirect.ActionName);
        }

        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(default(Product));

            var result = await _controller.Edit(productId);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product, resultProduct);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_InValidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gerekli");

            var result = _controller.Edit(productId, products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_IsValidModelState_ReturnRedirectToAction(int productId)
        {
            var result = _controller.Edit(productId, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(ProductsController.Index), redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_IsValidModelState_UpdateMethodExecute(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.Update(product));

            _controller.Edit(productId, product);

            _mockRepo.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async void Delete_IsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(default(Product));

            var result = await _controller.Delete(productId);

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Delete(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(ProductsController.Index), viewResult.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecutes(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.Delete(product));

            await _controller.DeleteConfirmed(productId);

            _mockRepo.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
        }
    }
}
