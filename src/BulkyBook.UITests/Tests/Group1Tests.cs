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
            var contentManagementLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Content Mangement')]")));
            contentManagementLink.Click();

            // Wait for Product link in dropdown to be clickable
            var productLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@class,'dropdown-item') and contains(text(),'Product')]")));
            productLink.Click();

            // Wait for the product row with 'xya' to be present and clickable
            var productRowLink = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//tr/td[contains(text(),'xya')]/parent::tr/td/div/a[1]")));
            productRowLink.Click();

            // Wait for ISBN input field to be present and send keys
            var isbnInput = _wait.Until(ExpectedConditions.ElementExists(By.Id("Product_ISBN")));
            isbnInput.Clear();
            isbnInput.SendKeys("1234567123");
        }

        [TearDown]
        public void TearDown()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}
