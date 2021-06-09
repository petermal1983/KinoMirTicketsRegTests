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
        protected IWebDriver driver; //Создаем экземпляр IWebDriver

        //XPath, используемые для поиска элементов
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
        private readonly By _chosenTicketsPlaceInfo = By.XPath("//div[@class='chosen-tickets__place']");
        private readonly By _chosenCinemaName = By.XPath("//div[@class='ticket-box__title']");
       private readonly By _ticketBoxInfo = By.XPath("//div[@class='ticket-box__info']");

        public static bool flag = false; //Переменная-флаг для использовния в функции определения даты при выборе сегодняшнего или завтрашнего сеанса



        [SetUp]
        public void TestInitialize() //Предварительные действия перед тестированием
        {
            driver = new OpenQA.Selenium.Chrome.ChromeDriver();
            driver.Navigate().GoToUrl("https://www.kino-mir.ru");
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //Неявное ожидание для выполнения методов в установленный таймаут
        }

        [Test]

        public void testing_Site()
        {

            close_City_Chooser(); //Закрываем всплывающее окно выбора города
            proceed_To_Cinemas_Schedule(); //Переходим ко вкладке "Расписание"
            close_Schedule_Popup(); //Закрываем всплывающее окно рекламы фильма
            select_Available_Sinema(); //Выбираем доступный сеанс
            select_if_VIP(); //Выбираем, явдяется ли выбранный зал VIP-залом, а также являются ли выбранные места местами Love Seats,
                              // т.к. в данном случае одним кликом выбираестя сразу 2 места.
            click_Agreement_Checkbox(); //Кликаем на подтверждении "Соглашения пользователя"
            click_Sumbit_Button(); // Нажимаем кнопку перехода к оплате
            check_Not_Null(); // Проверяем наличие данных билета
            check_If_True(); // Проверяем данные по заданным условиям  - текущая или завтрашняя дата, в зависимости от выбора билета,
                             //значение флага, наличие неизменяемого текста - требует доработки.
            

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
                selectCinema = driver.FindElement(_cinemasAvailable); //Выбор сеанса сегодня, если доступен
            }
            catch (NoSuchElementException)
            {

            }
            if (selectCinema != null)
            {
                selectCinema.Click();
                flag = true;
            }
            else
            {
                var goTomorrowCinemas = driver.FindElement(_nextDayButton); //Переход в раздел сеансов на завтра
                goTomorrowCinemas.Click();

                selectCinema = driver.FindElement(_cinemasAvailable); //Выбор завтрашнего сеанса
                selectCinema.Click();
            }
        }

        public void select_Seats()
        {
            var selectSeating = driver.FindElement(_seatSelect);
            selectSeating.Click();

        }

        public bool check_If_Love_Seats() { 
            try
            {
                driver.FindElement(_loveSeatSelector); //Определение, является ли место Love Seat
                return true;
            }
            catch (NoSuchElementException)
            { return false;
            
            }
               
         }

        public void select_if_VIP()
        {
            var select = driver.FindElement(_vipSelector); //Определение, является ли место местом VIP


            if (select.Text.Contains("ВИП") ||
                select.Text.Contains("VIP") ||
                check_If_Love_Seats() == true)
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
                actions.MoveToElement(selectedCheckBox, 10, 10).Click(); //Используем данные способ, т.к. прямой Click перехватывается
                actions.Build().Perform();

            }
        }

        public void click_Sumbit_Button()
        {
            var clickSubmit = driver.FindElement(_submitButton);
            Actions actions = new Actions(driver);
            actions.MoveToElement(clickSubmit, 1, 1).Click();
            actions.Build().Perform();
            clickSubmit.Submit(); //Такой способ срабатывает
            clickSubmit.Submit(); //Такой способ срабатывает
        }

        public void check_Not_Null() //Максимально простая проверка
        {
            var ticketBoxInf = driver.FindElement(_ticketBoxInfo).Text;
            Assert.IsNotNull(ticketBoxInf, "Sorry, an error occurred, your ticket info is empty.");
            var ticketsPlacesData = driver.FindElement(_chosenTicketsPlaceInfo).Text;
            Assert.IsNotNull(ticketsPlacesData, "Sorry, an error occurred, your places info is empty.");

        }

        public bool check_Contains_Tickets_Data() //Проверка наличия дат (сегодня, завтра) в билетах, и неизменяемого текста - требует доработки.
        {

           var month_and_date_Today = DateTime.Now.ToLongDateString().Substring(0, DateTime.Now.ToLongDateString().Length - 8);
            var month_and_date_Tomorrow = DateTime.Now.AddDays(1).ToLongDateString().Substring(0, DateTime.Now.ToLongDateString().Length - 8);
            string[] itemsToContain = { month_and_date_Today, month_and_date_Tomorrow, ""};

            if (driver.FindElement(_ticketBoxInfo).Text.Contains(month_and_date_Today) 
                && driver.FindElement(_ticketBoxInfo).Text.Contains("зал, в формат")
                && flag==true)
               
            {
                return true;
            }
               else if (driver.FindElement(_ticketBoxInfo).Text.Contains(month_and_date_Tomorrow) 
                && driver.FindElement(_ticketBoxInfo).Text.Contains("зал, в формат") 
                && flag==false)
            {
                return true;
            } 
            else
            {
                return false;
            }
            
        }


        public void check_If_True() {

            Assert.IsTrue(check_Contains_Tickets_Data());
        }

        [TearDown]
        public void TestCleanup()
        {

            try
            {
               driver.Quit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error closing driver: {ex.Message}");
            }
        }
    }
}