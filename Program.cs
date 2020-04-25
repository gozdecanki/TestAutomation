using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Web;

namespace TestAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChromeDriver chromeDriver= new ChromeDriver())
            {
                chromeDriver.Navigate().GoToUrl("https://www.gittigidiyor.com");
                //sayfanın yüklendiğinden emin olmamız gerekiyor
                WaitForPageLoad(chromeDriver);

                var policyAlertCloseButtonElement = chromeDriver.FindElementByClassName("policy-alert-close");
                policyAlertCloseButtonElement.Click();

                //giriş butonunu buluyoruz
                var loginButtonElements = chromeDriver.FindElementsByClassName("profile-name");           

                foreach (IWebElement loginButtonElement in loginButtonElements)
                {
                    if(loginButtonElement.Text=="Giriş Yap")
                    {
                        loginButtonElement.Click();
                    }

                }

                WaitForPageLoad(chromeDriver);


                var emailTextElement = chromeDriver.FindElementById("L-UserNameField");
                var passwordTextElement = chromeDriver.FindElementById("L-PasswordField");
                var loginFormButtonElement = chromeDriver.FindElementById("gg-login-enter");
                

                emailTextElement.SendKeys("mail adresinizi giriniz");
                passwordTextElement.SendKeys("şifrenizi giriniz");
                loginFormButtonElement.Click();

                WaitForPageLoad(chromeDriver);

                //sitede arama yaptırıyoruz
                var searchTextElement = chromeDriver.FindElementById("search_word");
                searchTextElement.SendKeys("samsung");

                var searchButtonElement = chromeDriver.FindElementById("header-search-find-link");
                searchButtonElement.Click();
                WaitForPageLoad(chromeDriver);

                var searchKeywordSpanElement = chromeDriver.FindElementByClassName("search-result-keyword");
                var searchResultCountSpanElement= chromeDriver.FindElementByClassName("result-count");
                string searchKeyword = searchKeywordSpanElement.Text;
                string searchResultCount = searchResultCountSpanElement.Text;
                Console.WriteLine("arama sonucu>>> Keyword "+ searchKeyword + " Kayıt sayısı: "+ searchResultCount);

                ////arama yaptıktan sonra pagination varsa 100den fazla ürün
                var page2Element = chromeDriver.FindElementByXPath("//*[@id=\"best-match-right\"]/div[5]/ul/li[2]/a");

                if (page2Element !=null && page2Element.Displayed)
                {
                    Console.WriteLine("2.sayfa var");
                    page2Element.Click();
                    WaitForPageLoad(chromeDriver);


                    List<String> productIds = new List<string>();
                    for (int i=0; i<3 ; i++)
                    {
                        var productElements = chromeDriver.FindElementsByClassName("catalog-seem-cell");
                        var productElement = productElements[i];
                        productElement.Click();
                        WaitForPageLoad(chromeDriver);

                        IWebElement favHolderElement = chromeDriver.FindElementById("spp-watch-product");
                        IWebElement favoriteButtonElement = favHolderElement.FindElement(By.ClassName("circleBtn"));
                       

                         if (!favoriteButtonElement.GetAttribute("class").Contains("selected")) /// circleBtn selected
                        {
                            favoriteButtonElement.Click();

                        }
                     

                        string productId = chromeDriver.Url.Split('=')[1];
                        productIds.Add(productId);
                      

                        chromeDriver.Navigate().Back();
                        WaitForPageLoad(chromeDriver);
                    }

                    chromeDriver.Navigate().GoToUrl("https://www.gittigidiyor.com/hesabim/izlediklerim");
                    WaitForPageLoad(chromeDriver);

                    foreach (var productId in productIds)
                    {
                        if (chromeDriver.PageSource.Contains(productId))
                        {
                            Console.WriteLine(productId+" ürün favorilerde bulundu");
                        }
                        else
                        {
                            Console.WriteLine( "ürün favorilerde bulunamadı");
                        }
                    }

                    //favdan çıkarma
                    IJavaScriptExecutor javascript = chromeDriver as IJavaScriptExecutor;
                    javascript.ExecuteScript("$(\".favorite-product-item\").click();");

                    var deleteButtonElement = chromeDriver.FindElementByClassName("robot-delete-all-button");
                    deleteButtonElement.Click();
                    WaitForPageLoad(chromeDriver);

                    chromeDriver.Navigate().GoToUrl("https://www.gittigidiyor.com");
                    WaitForPageLoad(chromeDriver);

                }
                else
                {
                    Console.WriteLine("2.sayfa yok");
                }
                Console.ReadLine();

            }
        }

        public static void WaitForPageLoad(IWebDriver webDriver)
        {
            TimeSpan timeout = new TimeSpan(0, 0, 60);
            WebDriverWait wait = new WebDriverWait(webDriver, timeout);
            IJavaScriptExecutor javascript = webDriver as IJavaScriptExecutor;
            if (javascript == null) throw new ArgumentException("driver", "driver must support javascript.");

            wait.Until((d) =>
            {
            try
            {
                string readyState = javascript.ExecuteScript("if(document.readyState) return document.readyState;").ToString();
                return readyState.ToLower() == "complete";
            }
            catch (InvalidOperationException ex)
            {
                    return ex.Message.ToLower().Contains("unable to get browser");


            }
            catch (WebDriverException ex)
            {
                return ex.Message.ToLower().Contains("unable to connect");
            }
            catch (Exception e)
            {
                return false;
            }
            });
            
        }
    }
}
