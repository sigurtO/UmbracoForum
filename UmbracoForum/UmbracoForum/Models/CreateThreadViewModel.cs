using System.ComponentModel.DataAnnotations;

namespace UmbracoForum.Models
{
    public class CreateThreadViewModel
    {
        [Required(ErrorMessage = "Please enter a title.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter your question or discussion.")]
        public string Content { get; set; }

        // We need this so Umbraco knows WHICH topic folder to save this thread under!
        public int TopicId { get; set; }
    }
}
