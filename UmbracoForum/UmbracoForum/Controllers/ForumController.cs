using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using UmbracoForum.Models;
using Umbraco.Cms.Core.Models;


namespace UmbracoForum.Controllers
{
    public class ForumController : SurfaceController
    {
        private readonly IContentService _contentService;

        public ForumController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IContentService contentService)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            // We inject the ContentService so we can create nodes programmatically
            _contentService = contentService;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitNewThread(CreateThreadViewModel model)
        {
            // 1. Check if the form is valid (e.g., they didn't leave the title blank)
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            // 2. Create the new node in memory
            var newThread = _contentService.Create(model.Title, model.TopicId, "forumThread");

            // 3. Set your custom properties
            newThread.SetValue("threadTitle", model.Title);
            newThread.SetValue("threadContent", model.Content);

            // 4. Save the content to the database first
            var saveResult = _contentService.Save(newThread);

            if (saveResult.Success)
            {
                // 5. Publish it to the live site ("*" means publish to all languages/cultures)
                var publishResult = _contentService.Publish(newThread, new[] { "*" });

                if (publishResult.Success)
                {
                    // If it worked, redirect the user directly to the new page they just created!
                    var publishedNode = UmbracoContext.Content?.GetById(newThread.Id);
                    if (publishedNode != null)
                    {
                        return Redirect(publishedNode.Url());
                    }
                }
            }

            // Fallback if something goes horribly wrong
            TempData["ErrorMessage"] = "Something went wrong creating your thread.";
            return CurrentUmbracoPage();
        }

    }
}
