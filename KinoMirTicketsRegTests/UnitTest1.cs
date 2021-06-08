using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Diagnostics;

namespace KinoMirTicketsRegTests
{
    [TestFixture]
    public class Tests
    {
        protected IWebDriver driver;

        private readonly By _geoChooserCloser = By.XPath("//a[@class='geo-chooser__closer' or @href='#']");
        private readonly By _cinemasPath = By.XPath("//a[@href='/cinemas']");
        private readonly By _popupCloser = By.XPath("//a[@class='popup-notification__closer']");
        private readonly By _cinemasAvailable = By.XPath("//a[@class='diagram__sessions__item js-diagram__sessions__item']");
        private readonly By _nextDayButton = By.XPath("//a[text()='Завтра']");
        private readonly By _vipSelector = By.XPath("//div[@class='ticket-box__info']");
        private readonly By _seatSelect = By.XPath("//div[@class='hall__place state-0' or " +
                                                   "@class='hall__place state-0 hall__place_imax_standard' or " +
            "                                       @class='hall__place state-0 hall__place_dbox' or " +
            "                                       @class ='hall__place state-0 hall__place_live']");
        private readonly By _loveSeatSelector = By.XPath("//div[@data-section-name='LoveSeat']");
        private readonly By _checkBoxSelect = By.XPath("//div[@class='input checkbox' or" +
            "                                            @class='checkbox_tick fs-12 mb-14 js-agreement-wrapper']");
        private readonly By _submitButton = By.XPath("//button[@name='submit' or @type='submit']");
        private readonly By _ticketBoxInfo = By.XPath("//div[@class='ticket-box__info']");
        private readonly By _chosenTicketsPlaceInfo = By.XPath("//div[@class='chosen-tickets__place']");
        private readonly By _chosenCinemaName = By.XPath("//div[@class='ticket-box__title']");



        [SetUp]
        public void TestInitialize()
        {
            driver = new OpenQA.Selenium.Chrome.ChromeDriver();
            driver.Navigate().GoToUrl("https://www.kino-mir.ru");
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]

        public void testing_Site()
        {

            close_City_Chooser();
            proceed_To_Cinemas_Schedule();
            close_Schedule_Popup();
            select_Available_Sinema();
            select_if_VIP();
            click_Agreement_Checkbox();
            click_Sumbit_Button();
            check_Not_Null();
            check_Equal_Tickets_Data();

        }

        public void close_City_Chooser()
        {
            var closeChooser = driver.FindElement(_geoChooserCloser);

            closeChooser.Click();
        }

        public void proceed_To_Cinemas_Schedule()
        {
            var goToSchedule = driver.FindElement(_cinemasPath);
            goToSchedule.Click();
        }

        public void close_Schedule_Popup()
        {
            var closePopup = driver.FindElement(_popupCloser);
            closePopup.Click();
        }

        public void select_Available_Sinema()
        {

            IWebElement selectCinema = null;
            try
            {
                selectCinema = driver.FindElement(_cinemasAvailable);
            }
            catch (NoSuchElementException)
            {

            }
            if (selectCinema != null)
            {
                selectCinema.Click();
            }
            else
            {
                var goTomorrowCinemas = driver.FindElement(_nextDayButton);
                goTomorrowCinemas.Click();

                selectCinema = driver.FindElement(_cinemasAvailable);
                selectCinema.Click();
            }
        }

        public void select_Seats()
        {
            var selectSeating = driver.FindElement(_seatSelect);
            selectSeating.Click();

        }

        public bool check_exists_by_xpath() {
            try
            {
                driver.FindElement(_loveSeatSelector);
                return true;
            }
            catch (NoSuchElementException)
            { return false;
            
            }
               
         }

        public void select_if_VIP()
        {
            var select = driver.FindElement(_vipSelector);


            if (select.Text.Contains("ВИП") ||
                select.Text.Contains("VIP") ||
                check_exists_by_xpath() == true)
            {
                select_Seats();
            }

            else
            {
                select_Seats();
                select_Seats();
            }
        }

        public void click_Agreement_Checkbox()
        {

            if (!driver.FindElement(_checkBoxSelect).Selected)
            {
                var selectedCheckBox = driver.FindElement(_checkBoxSelect);
                Actions actions = new Actions(driver);
                actions.MoveToElement(selectedCheckBox, 10, 10).Click();
                actions.Build().Perform();

            }
        }

        public void click_Sumbit_Button()
        {
            var clickSubmit = driver.FindElement(_submitButton);
            Actions actions = new Actions(driver);
            actions.MoveToElement(clickSubmit, 1, 1).Click();
            actions.Build().Perform();
            clickSubmit.Submit();
        }

        public void check_Not_Null()
        {
            var ticketBoxInf = driver.FindElement(_ticketBoxInfo).Text;
            Assert.IsNotNull(ticketBoxInf, "Sorry, an error occurred, your ticket info is empty.");
            var ticketsPlacesData = driver.FindElement(_chosenTicketsPlaceInfo).Text;
            Assert.IsNotNull(ticketsPlacesData, "Sorry, an error occurred, your places info is empty.");

        }

        public void check_Equal_Tickets_Data()
        {


        }



        [TearDown]
        public void TestCleanup()
        {

            try
            {
               // driver.Quit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error closing driver: {ex.Message}");
            }
        }
    }
}