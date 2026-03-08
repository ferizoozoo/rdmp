using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Services;

public interface ICrawlerService
{
    Task<string> CrawlJobPostingAsync(string url);
}

public class CrawlerService : ICrawlerService
{
    public async Task<string> CrawlJobPostingAsync(string url)
    {
        try
        {
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl(url);

            var title = driver.Title;

            var jobDescription = driver.FindElement(By.Id("jobDescriptionText"));
            var jobDescriptionText = jobDescription.GetAttribute("innerText");

            if (jobDescriptionText == string.Empty)
            {
                driver.Quit();
                throw new Exception("Job description element not found.");
            }

            driver.Quit();

            return jobDescriptionText;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error crawling job posting: {ex.Message}");
            return string.Empty;
        }
    }
}