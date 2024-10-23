using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace UiTests;

[TestClass]
public class FeedbackTests : PageTest
{
    [TestMethod]
    public async Task WhenCommentsProvided_SubmitSuccessful()
    {
        await Page.GotoAsync("/comments");

        var commentsArea = Page.Locator("textarea[id='comments-area']");
        await commentsArea.FillAsync($"Comment from WhenCommentsProvided_SubmitSuccessful - {DateTime.UtcNow}");

        await Page.ClickAsync("button[id='submit-btn']");

        await Expect(Page.Locator("p[id='comment-confirmation']")).ToBeVisibleAsync();
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            BaseURL = TestContext.Properties["baseUrl"]!.ToString(),
        };
    }
}