namespace ImageGalleryDemo
{
    // Help store the image json data.
    public class ImageItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public byte[] Base64 { get; set; }
        public string Format { get; set; }
    }
}
