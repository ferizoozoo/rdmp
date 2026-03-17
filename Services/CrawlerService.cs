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
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            var driver = new ChromeDriver(options);

            driver.Navigate().GoToUrl(url);

            var title = driver.Title;

            // TODO: the main issue here is that for using on many links, each has its own class or id 
            var jobDescription = driver.FindElement(By.ClassName("job-posting-details-body"));
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