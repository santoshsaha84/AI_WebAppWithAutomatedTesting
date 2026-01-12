using BulkyBook.UITests.Helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace BulkyBook.UITests.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class CrudTests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private const string AppUrl = "http://localhost:5003/";

        [SetUp]
        public void SetUp()
        {
            // Reset database to "Bulky" which is what localhost:5003 uses
            DatabaseHelper.ResetDatabaseToKnownState("Bulky");
            
            var options = new ChromeOptions();
            if (Environment.GetEnvironmentVariable("HEADLESS") == "true")
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--window-size=1920,1080");
            }
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
        }

        [Test]
        [Category("CRUD")]
        public void AddCategory_Success()
        {
            _driver.Navigate().GoToUrl(AppUrl);
            _driver.Manage().Window.Maximize();

            // Navigate to Category
            var contentManagementLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Content Mangement')]")));
            contentManagementLink.Click();
            var categoryLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@class,'dropdown-item') and contains(text(),'Category')]")));
            categoryLink.Click();

            // Create New Category
            var createBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Create New Category')]")));
            createBtn.Click();

            _wait.Until(ExpectedConditions.ElementExists(By.Id("Name"))).SendKeys("UI Test Category");
            _driver.FindElement(By.Id("DisplayOrder")).SendKeys("99");
            _driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            // Verify
            _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[text()='UI Test Category']")));
        }

        [Test]
        [Category("CRUD")]
        public void DeleteCategory_Success()
        {
            _driver.Navigate().GoToUrl(AppUrl);
            
            // Navigate to Category
            var contentManagementLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Content Mangement')]")));
            contentManagementLink.Click();
            var categoryLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@class,'dropdown-item') and contains(text(),'Category')]")));
            categoryLink.Click();

            // Delete 'Math' category (seeded)
            var deleteBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//td[text()='Math']/parent::tr//a[contains(@class,'btn-danger')]")));
            deleteBtn.Click();

            // Confirm Delete
            var confirmDeleteBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@type='submit' and text()='Delete']")));
            confirmDeleteBtn.Click();

            // Verify removal
            _wait.Until(ExpectedConditions.InvisibilityOfElementWithText(By.XPath("//td"), "Math"));
        }

        [Test]
        [Category("CRUD")]
        public void AddProduct_Success()
        {
            _driver.Navigate().GoToUrl(AppUrl);

            // Navigate to Product
            var contentManagementLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Content Mangement')]")));
            contentManagementLink.Click();
            var productLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@class,'dropdown-item') and contains(text(),'Product')]")));
            productLink.Click();

            // Create New Product
            var createBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Create New Product')]")));
            createBtn.Click();

            _wait.Until(ExpectedConditions.ElementExists(By.Id("Product_Title"))).SendKeys("UI Test Product");
            _driver.FindElement(By.Id("Product_ISBN")).SendKeys("UT-123456");
            _driver.FindElement(By.Id("Product_Author")).SendKeys("Selenium Tester");
            _driver.FindElement(By.Id("Product_ListPrice")).SendKeys("50");
            _driver.FindElement(By.Id("Product_Price")).SendKeys("45");
            _driver.FindElement(By.Id("Product_Price50")).SendKeys("40");
            _driver.FindElement(By.Id("Product_Price100")).SendKeys("35");
            
            var categorySelect = new SelectElement(_driver.FindElement(By.Id("Product_CategoryId")));
            categorySelect.SelectByText("History");

            _driver.FindElement(By.XPath("//button[@type='submit' and text()='Create']")).Click();

            // Verify
            _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[text()='UI Test Product']")));
        }

        [Test]
        [Category("CRUD")]
        public void DeleteProduct_Success()
        {
            _driver.Navigate().GoToUrl(AppUrl);

            // Navigate to Product
            var contentManagementLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Content Mangement')]")));
            contentManagementLink.Click();
            var productLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@class,'dropdown-item') and contains(text(),'Product')]")));
            productLink.Click();

            // Delete 'xya' product (seeded)
            var deleteBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//td[text()='xya']/parent::tr//a[contains(@class,'btn-danger')]")));
            deleteBtn.Click();

            // Confirm Delete
            var confirmDeleteBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@type='submit' and text()='Delete']")));
            confirmDeleteBtn.Click();

            // Verify removal
            _wait.Until(ExpectedConditions.InvisibilityOfElementWithText(By.XPath("//td"), "xya"));
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                string screenshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
                Directory.CreateDirectory(screenshotPath);
                string fileName = $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMddHHmmss}.png";
                string fullPath = Path.Combine(screenshotPath, fileName);

                try
                {
                    var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                    screenshot.SaveAsFile(fullPath, ScreenshotImageFormat.Png);
                    Console.WriteLine($"[DEBUG] Screenshot saved to: {fullPath}");
                    Console.WriteLine("[DEBUG] Page Source at failure:");
                    Console.WriteLine(_driver.PageSource);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEBUG] Failed to capture screenshot: {ex.Message}");
                }
            }

            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}
