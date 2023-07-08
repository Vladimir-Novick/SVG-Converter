using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Svg;


using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Media.Media3D;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Controls;
//using System.Windows.Shapes;

/// <summary> 
/// <Library="SVG"> This is the 3rd Party library compiled from : https://github.com/vvvv/SVG </Library>
/// <Icon="Icon">https://www.iconfinder.com/icons/88561/filetype_png_icon#size=128</Icon>
/// <Button_ContextIcons>https://www.iconfinder.com/iconsets/gnome-desktop-icons-png</Button_ContextIcons>
/// </summary>

namespace SVG_Converter
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// The Base Title of the form.
        /// </summary>
        private string Title
        {
            get
            {
                return "SVG Converter";
            }
        }




        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 4;

            backgroundWorker1.DoWork +=
               new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler
                   (backgroundWorker1_ProgressChanged);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Scalable Vector Graphics File (*.svg)|*.svg", Multiselect = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    for (int i = 0; i <= ofd.FileNames.Length - 1; i++)
                    {
                        // here we convert the svg and put to the bitmap.
                        using (Bitmap B = SvgDocument.Open(ofd.FileNames[i]).Draw())
                        {
                            // adding the current image to imagelist.
                            AddImageToImageList(imageList1, Path.GetFileName(ofd.FileNames[i]), B);
                            // getting the svg filename.
                            string filename = Path.GetFileName(ofd.FileNames[i]);
                            // adding the current image to listview.
                            AddItemToListView(listView1, filename, new string[] { ofd.FileNames[i] }, imageList1);
                        }
                    }
            }

            Text = Title + string.Format(" : Items ({0}).", listView1.Items.Count);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var FBD = new FolderBrowserDialog())
            {
                if (FBD.ShowDialog() == DialogResult.OK)
                    for (int i = 0; i <= listView1.Items.Count - 1; i++)
                    {
                        // here we convert the svg and put to the bitmap.
                        using (Bitmap B = SvgDocument.Open(listView1.Items[i].SubItems[1].Text).Draw())
                        {
                            // We get the same svg file names and put to the png files.
                            B.Save(FBD.SelectedPath + "/" + listView1.Items[i].Text.Replace(".svg", "." + comboBox1.SelectedItem.ToString()), ImgFormat(comboBox1.SelectedItem.ToString()));
                        }
                    }
                // here we show the number of svg files converted.
                MessageBox.Show(string.Format("{0} SVG Items Successfully Converted To PNG !", listView1.Items.Count) + Environment.NewLine + string.Format("Destination : {0}", FBD.SelectedPath));
            }

        }

        /// <summary>
        /// Adds images to Imagelist
        /// </summary>
        /// <param name="imgList">the ImageList to add images</param>
        /// <param name="imgName">name for the image in ImageList</param>
        /// <param name="img">the image to add</param>
        private void AddImageToImageList(ImageList imgList, string imgName, System.Drawing.Image img)
        {
            imgList.Images.Add(imgName, img);
        }

        /// <summary>
        /// Gets the ImageIndex in ImageList
        /// </summary>
        /// <param name="imgList">the ImageList which contains the the image to get index</param>
        /// <param name="keyName">the image name inside the ImageList</param>
        /// <returns></returns>
        private int ImageListImageIndex(ImageList imgList, string keyName)
        {
            return imgList.Images.IndexOfKey(keyName);
        }

        /// <summary>
        /// Adds and item to listview cotnrol.
        /// </summary>
        /// <param name="listView">The ListView to storing items.</param>
        /// <param name="item">An Item to add.</param>
        /// <param name="subitems">The Subitems of current item.</param>
        /// <param name="imageList">The Imagelist for the listview.</param>
        private void AddItemToListView(System.Windows.Forms.ListView listView, string item, string[] subitems, ImageList imageList)
        {
            var LVI = new System.Windows.Forms.ListViewItem();
            LVI.Text = item;
            LVI.SubItems.AddRange(subitems); //
            LVI.ImageIndex = ImageListImageIndex(imageList, item);
            listView.Items.Add(LVI);
        }


        /// <summary>
        /// The Method to clear listview and returing the base title to the form.
        /// </summary>
        private void Clear()
        {
            // Clearing the items of the listview.
            listView1.Items.Clear();
            // returing the base title to the form.
            Text = Title;
        }


        /// <summary>
        /// The Method to return the ImageFormat based om limited ImageFormats in string.
        /// </summary>
        /// <param name="str">The ImageFormat in string.</param>
        /// <returns>The Imageformat base on given string</returns>
        private ImageFormat ImgFormat(string str)
        {
            switch (str)
            {
                case "Bmp":
                    return ImageFormat.Bmp;
                case "Gif":
                    return ImageFormat.Gif;
                case "Jpeg":
                    return ImageFormat.Jpeg;
                case "Png":
                    return ImageFormat.Png;
                case "Tiff":
                    return ImageFormat.Tiff;
                case "Icon":
                    return ImageFormat.Icon;
                default:
                    return ImageFormat.Png;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "D:";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBoxSVG.Text = dialog.FileName;

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "D:";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBoxTagret.Text = dialog.FileName;

            }
        }

        private static SvgDocument LoadSvgDocument(string svgPath)
        {
            try
            {
                return SvgDocument.Open(svgPath);
            }
            catch (Exception)
            {
                return null;
            }
        }


        struct argorker_type
        {
            public string sourceFolder;
            public string targrtFolder;
            public string imageType;
            public float withSize;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                 textBoxLOG.Text = "***** Canceled! *****";
            }
            else if (e.Error != null)
            {
              
                textBoxLOG.Text = "..... Error: " + e.Error.Message;
            }
            else
            {
                textBoxLOG.Text = "..... Complete .....";
            }


           
            BtnPackageConvert.Enabled = true;
            progressBar1.Value = 0;
        }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BackgroundWorker helperBW = sender as BackgroundWorker;
            string fileName = (string)e.UserState;

            textBoxLOG.Text = fileName;
            progressBar1.Value = e.ProgressPercentage ;

        }

        private bool backgroundWorker1_cansel = false;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            argorker_type arg = (argorker_type)e.Argument;
            BackgroundWorker helperBW = sender as BackgroundWorker;
            string sourceFolder = arg.sourceFolder;
            string targrtFolder = arg.targrtFolder;
            float Width = arg.withSize;
            DirectoryInfo d = new DirectoryInfo(sourceFolder);
            {

                FileInfo[] Files = d.GetFiles("*.svg"); //Getting Text files

                int i = 1;
                int filesSize = Files.Length;

                foreach (FileInfo file_ in Files)
                {
                    string file = file_.FullName;
                    var fileName = Path.GetFileName(file);
                    int percent = Convert.ToInt32(((float)i / (float)filesSize) * 100);
                    i++;

                    helperBW.ReportProgress(percent, fileName);
                    {


                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
               
                            return;
                        }


                        var doc = LoadSvgDocument(file);

                        try
                        {

                            if (doc != null)
                            {
                                if (arg.withSize != 0)
                                {
                                    doc.ShapeRendering = SvgShapeRendering.Auto;

                                    int imageWidth = Convert.ToInt32(doc.Width.Value);
                                    int imageHeight = Convert.ToInt32(doc.Height.Value);
                                    float ratio = (float)arg.withSize / (float)imageWidth;

                                    int WidthImage = Convert.ToInt32(arg.withSize);
                                    int HeightImage = Convert.ToInt32(imageHeight * ratio);
                                    doc.Width = WidthImage;
                                    doc.Height = HeightImage;
                                }

                                using (Bitmap B = doc.Draw())
                                {

                                    B.Save(targrtFolder + "/" + fileName.Replace(".svg", "." + arg.imageType), ImgFormat(arg.imageType));

                                }

                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }

        }



        private void BtnPackageConvert_Click(object sender, EventArgs e)
        {
            var sourceFolder = textBoxSVG.Text;
            var targrtFolder = textBoxTagret.Text;
            argorker_type arg;


            DialogResult dialogResult = MessageBox.Show(sourceFolder + "  \n to \n" + targrtFolder, "Are You Sure convert ?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                progressBar1.Maximum = 100;
                progressBar1.Step = 1;
                progressBar1.Value = 0;
                arg.sourceFolder = textBoxSVG.Text;
                arg.targrtFolder = textBoxTagret.Text;
                arg.imageType = comboBox1.SelectedItem.ToString();

                string strSize = textBoxImageSize.Text;
                float intSize;
                float.TryParse(strSize, out intSize);
                arg.withSize = intSize;

                if (!backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.RunWorkerAsync(arg);
                    BtnPackageConvert.Enabled = false;
                    backgroundWorker1_cansel = false;
                }
            }
            else if (dialogResult == DialogResult.No)
            {

            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }
    }
}