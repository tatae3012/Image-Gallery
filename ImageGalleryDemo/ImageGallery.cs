using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using C1.Win.C1Tile;

namespace ImageGalleryDemo
{
    public partial class ImageGallery : Form
    {
        // Instance of Datafetch class created.
        DataFetcher datafetch = new DataFetcher();
        // List of ImageItem class declared.
        List<ImageItem> imagesList;
        // Number of checked objects are initially 0.
        int checkedItems = 0;
        public ImageGallery()
        {
            InitializeComponent();
        }

        // A gray highlight is created whenever the seach panel is painted.
       private void OnSearchPanelPaint(object sender, PaintEventArgs e)
       {
            // Get the dimensions of the box.
            Rectangle r = _searchBox.Bounds;
            // Inflate the boundaries by mentioned values.
            r.Inflate(3, 3);
            Pen p = new Pen(Color.LightGray);
            // Draw a gray highlight around the box.
            e.Graphics.DrawRectangle(p, r);
       }

        // The method loops through all the images and add it to the tile control.
        private void AddTiles(List<ImageItem> imageList)
        {
            // Clear all tiles if already in use.
            _imageTileControl.Groups[0].Tiles.Clear();
            foreach (var imageitem in imageList)
            {
                Tile tile = new Tile();
                tile.HorizontalSize = 2;
                tile.VerticalSize = 2;
                _imageTileControl.Groups[0].Tiles.Add(tile);
                // Convert the base64 encoding to the corresponding image.
                Image img = Image.FromStream(new
                MemoryStream(imageitem.Base64));
                Template tl = new Template();
                ImageElement ie = new ImageElement();
                // Fits the image to tile.
                ie.ImageLayout = ForeImageLayout.Stretch;
                tl.Elements.Add(ie);
                tile.Template = tl;
                tile.Image = img;
            }
        }

        // When search button is clicked, we fetch the images using DataFetcher class.
        private async void OnSearchClick(object sender, EventArgs e)
        {
            statusStrip1.Visible = true;
            imagesList = await
            // Fetches the images.
            datafetch.GetImageData(_searchBox.Text);
            // The fetched data is added to tiles in the form of ImageItem.
            AddTiles(imagesList);
            statusStrip1.Visible = false;
        }

        // The method converts the input list of images to a pdf.
        private void ConvertToPdf(List<Image> images)
        {
            RectangleF rect = _imagePdfDocument.PageRectangle;
            bool firstPage = true;
            // Go through all the images in the List.
            foreach (var selectedimg in images)
            {
                if (!firstPage)
                {
                    // If the page is filled, call new page.
                    _imagePdfDocument.NewPage();
                }
                firstPage = false;
                // Size of the rectangle is reduced.
                rect.Inflate(-72, -72);
                // The image in teh form of rec is drawn on pdf document. 
                _imagePdfDocument.DrawImage(selectedimg, rect);
            }

        }

        // Function when pdf export button is clicked. 
        private void OnExportClick(object sender, EventArgs e)
        {
            // A list to store all the images is created.
            List<Image> images = new List<Image>();
            foreach (Tile tile in _imageTileControl.Groups[0].Tiles)
            {
                if (tile.Checked)
                {
                    // If tile is checked then that image is added to the list.
                    images.Add(tile.Image);
                }
            }
            // The convert function is called with the list parameter.
            ConvertToPdf(images);
            // After the pdf is created, a save dialog box is displayed to finally save that pdf.
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = "pdf";
            saveFile.Filter = "PDF files (*.pdf)|*.pdf*";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                // If OK is selected, the pdf will be saved int he selected location.
                _imagePdfDocument.Save(saveFile.FileName);
            }
        }


        // Highlight the export button with gray color.
        private void OnExportImagePaint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(_exportImage.Location.X, _exportImage.Location.Y, _exportImage.Width, _exportImage.Height);
            r.X -= 29;
            r.Y -= 3;
            r.Width--;
            r.Height--;
            Pen p = new Pen(Color.LightGray);
            e.Graphics.DrawRectangle(p, r);
            // Finally draws the required line on the pdf picture box.
            e.Graphics.DrawLine(p, new Point(0, 43), new Point(this.Width, 43));
        }

        // Increment the counter whenever more tiles are selected.
        private void OnTileChecked(object sender, TileEventArgs e)
        {
            checkedItems++;
            // The pdf convert button will be visible.
            _exportImage.Visible = true;
        }

        // Deccrement the counter whenever a tile are un-selected.
        private void OnTileUnchecked(object sender, TileEventArgs e)
        {
            checkedItems--;
            // The pdf convert button will be visible only if atleast one tile is selected.
            _exportImage.Visible = checkedItems > 0;
        }

        // To keep a track of how many tiles are selected.
        private void OnTileControlPaint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.LightGray);
            // If the tile is selected, a border is drawn to highlight it.
            e.Graphics.DrawLine(p, 0, 43, 800, 43);
        }
        private void ImageGallery_Load(object sender, EventArgs e)
        {
             
        }
    }
}
