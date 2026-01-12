using BulkyBook.UITests.Helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace BulkyBook.UITests.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class VendorTests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private const string AppUrl = "http://localhost:5004/";

        [SetUp]
        public void SetUp()
        {
            DatabaseHelper.ResetDatabaseToKnownState("Bulky_1");
            
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
        public void AddVendor_Success()
        {
            _driver.Navigate().GoToUrl(AppUrl);
            _driver.Manage().Window.Maximize();

            var vendorMgmtLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(.,'Vendor Management')]")));
            vendorMgmtLink.Click();
            var vendorAddressLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@class,'dropdown-item') and contains(.,'Vendor Address')]")));
            vendorAddressLink.Click();

            var createBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(.,'Create New Vendor')]")));
            createBtn.Click();

            _wait.Until(ExpectedConditions.ElementExists(By.Id("Name"))).SendKeys("UI Test Vendor");
            _driver.FindElement(By.Id("City")).SendKeys("Test City");
            _driver.FindElement(By.Id("Address")).SendKeys("Test Address");
            
            var submitBtn = _driver.FindElement(By.XPath("//button[contains(@type,'submit') and contains(.,'Create')]"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitBtn);

            _wait.Until(ExpectedConditions.ElementExists(By.XPath("//td[contains(.,'UI Test Vendor')]")));
        }

        [Test]
        public void DeleteVendor_Success()
        {
            _driver.Navigate().GoToUrl(AppUrl);
            
            var vendorMgmtLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(.,'Vendor Management')]")));
            vendorMgmtLink.Click();
            var vendorAddressLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@class,'dropdown-item') and contains(.,'Vendor Address')]")));
            vendorAddressLink.Click();

            var deleteBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//td[contains(.,'Seed Vendor')]/parent::tr//a[contains(@class,'btn-danger')]")));
            deleteBtn.Click();

            var confirmDeleteBtn = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(@type,'submit') and contains(.,'Delete')]")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", confirmDeleteBtn);

            _wait.Until(ExpectedConditions.InvisibilityOfElementWithText(By.XPath("//td"), "Seed Vendor"));
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
