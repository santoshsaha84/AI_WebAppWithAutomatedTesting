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
    public class Group1Tests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private const string AppUrl = "http://localhost:5003/";

        [SetUp]
        public void SetUp()
        {
            DatabaseHelper.ResetDatabaseToKnownState("Bulky");
            
            var options = new ChromeOptions();
            if (Environment.GetEnvironmentVariable("HEADLESS") == "true")
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--window-size=1920,1080"); // Ensure desktop layout in headless
            }
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
        }

        [Test]
        [Category("Group1")]
        public void TestFeature1()
        {
            _driver.Navigate().GoToUrl(AppUrl);
            _driver.Manage().Window.Maximize();

            // Wait for page to load - wait for navbar to be present
            _wait.Until(ExpectedConditions.ElementExists(By.ClassName("navbar")));
            
            // Wait for Content Management dropdown toggle (note: actual text is "Content Mangement" with typo)
            var contentManagementLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(.,'Content Mangement')]")));
            contentManagementLink.Click();

            // Wait for Product link in dropdown to be clickable
            var categoryLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@class,'dropdown-item') and contains(.,'Product')]")));
            categoryLink.Click();

            // Wait for the product row with 'xya' to be present and clickable
            var productRowLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//tr/td[contains(.,'xya')]//parent::tr//a[contains(@href, 'Upsert')]")));
            productRowLink.Click();

            // Wait for ISBN input field to be present and send keys
            var isbnInput = _wait.Until(ExpectedConditions.ElementExists(By.Id("Product_ISBN")));
            isbnInput.Clear();
            isbnInput.SendKeys("1234567123");
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
                    
                    // Also print page source for DOM analysis
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
