namespace Zermos_Web.Models
{
    public class InformatieBoordModel
    {
        string title;
        string subTitle;
        string image;
        string content;
        
        public string Title { get => title; set => title = value; }
        public string SubTitle { get => subTitle; set => subTitle = value; }
        public string Image { get => image; set => image = value; }
        public string Content { get => content; set => content = value; }
        
        public InformatieBoordModel(string title, string subTitle, string image, string content)
        {
            Title = title;
            SubTitle = subTitle;
            Image = image;
            Content = content;
        }
    }
}