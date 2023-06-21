namespace Zermos_Web.Models
{
    public class InformatieBoordModel
    {
        public InformatieBoordModel(string title, string subTitle, string image, string content)
        {
            Title = title;
            SubTitle = subTitle;
            Image = image;
            Content = content;
        }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string Image { get; set; }

        public string Content { get; set; }
    }
}