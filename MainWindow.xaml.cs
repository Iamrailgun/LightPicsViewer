using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace lpv
{

  public partial class MainWindow : Window
  {

    private string[] filesInDir;
    private string[] images;
    private int imgIdx = 0;
    private Vector2D imgSize;
    private Vector2D imgSizeTo;
    private Vector2D imgMargin;
    private Vector2D imgMarginTo;
    private Vector2D mouseImgPos;
    private Vector2D mousePos;
    private double maxImgHeight;
    private double scaleRate;
    private double resizeSpeed;
    private double moveSpeed;
    private double minImgHeight;
    private int refreshRate;
    private double maxImgHeightValue;
    private bool isMove = false;
    private Thread thread;
    private double deltaTime;
    private readonly string[] exts = { "bmp", "dib", "rle", "jpg", "jpeg", "jpe", "jfif", "gif", "emf", "wmf", "tif", "tiff", "png", "ico" };

    public MainWindow()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      scaleRate = Properties.Settings.Default.scaleRate;
      resizeSpeed = Properties.Settings.Default.resizeSpeed;
      moveSpeed = Properties.Settings.Default.moveSpeed;
      refreshRate = Properties.Settings.Default.refreshRate;
      maxImgHeightValue = Properties.Settings.Default.maxImgHeightValue;
      minImgHeight = Properties.Settings.Default.minImgHeight;
      Background.Opacity = Properties.Settings.Default.backgroundDim;
      var args = Application.Current.FindResource("args") as string[];
      try
      {
        filesInDir = Directory.GetFiles(Path.GetDirectoryName(args[0]));
      }
      catch (Exception)
      {
        Application.Current.Shutdown();
      }
      images = filesInDir.Where(f => exts.Contains(f.Split('.').Last())).ToArray();
      imgIdx = Array.IndexOf(images, args[0]);
      maxImgHeight = Height * maxImgHeightValue;
      deltaTime = 1.0 / refreshRate;
      ShowImage(images[imgIdx]);
      thread = new Thread(new ThreadStart(Updata));
      thread.Start();
    }

    private void Updata()
    {
      while (true)
      {
        Thread.Sleep(1000 / refreshRate);
        image.Dispatcher.Invoke(new Action(UpdateImage));
      }
    }
    private void UpdateImage()
    {
      if (isMove)
      {
        imgMarginTo = mousePos - mouseImgPos;
        imgMargin = imgMargin.Lerp(imgMarginTo, moveSpeed * deltaTime);
        image.Margin = new Thickness(imgMargin.X, imgMargin.Y, 0, 0);
        imgSizeTo = imgSize;
      }
      else
      {
        imgSize = imgSize.Lerp(imgSizeTo, resizeSpeed * deltaTime);
        image.Width = imgSize.X;
        image.Height = imgSize.Y;
        imgMargin = imgMargin.Lerp(imgMarginTo, resizeSpeed * deltaTime);
        image.Margin = new Thickness(imgMargin.X, imgMargin.Y, 0, 0);
      }
    }

    private void ShowImage(string imgPath)
    {
      try
      {
        var img = new BitmapImage(new Uri(imgPath));
        var aspectRatio = img.Height / img.Width;
        if (img.Height > maxImgHeight)
        {
          imgSize = new Vector2D(maxImgHeight / aspectRatio, maxImgHeight);
        }
        else
        {
          imgSize = new Vector2D(img.Width, img.Height);
        }
        if (img.Height < minImgHeight)
        {
          imgSize = new Vector2D(minImgHeight / aspectRatio, minImgHeight);
        }
        imgSizeTo = imgSize;
        image.Width = imgSize.X;
        image.Height = imgSize.Y;
        var left = Width / 2 - imgSize.X / 2;
        var top = Height / 2 - imgSize.Y / 2;
        imgMargin = new Vector2D(left, top);
        imgMarginTo = imgMargin;
        image.Margin = new Thickness(left, top, 0, 0);
        image.Source = img;
        image.Visibility = Visibility.Visible;
      }
      catch (Exception)
      {
        Application.Current.Shutdown();
      }
    }

    private void NextImage()
    {
      image.Visibility = Visibility.Hidden;
      imgIdx++;
      if (imgIdx > images.Length - 1)
      {
        imgIdx = 0;
      }
      ShowImage(images[imgIdx]);
    }

    private void PrevImage()
    {
      image.Visibility = Visibility.Hidden;
      imgIdx--;
      if (imgIdx < 0)
      {
        imgIdx = images.Length - 1;
      }
      ShowImage(images[imgIdx]);
    }

    private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      var miPos = new Vector2D(Mouse.GetPosition(image));
      var prevSize = imgSize;
      if (e.Delta > 0 && !isMove)
      {
        imgSizeTo = prevSize * (1 + scaleRate);
        imgMarginTo = imgMargin - (miPos * (1 + scaleRate) - miPos);
      }
      else if (imgSize.Y > minImgHeight)
      {
        imgSizeTo = prevSize * (1 - scaleRate);
        imgMarginTo = imgMargin - (miPos * (1 - scaleRate) - miPos);
      }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
      {
        Application.Current.Shutdown();
      }
      if (e.Key == Key.Right)
      {
        NextImage();
      }
      if (e.Key == Key.Left)
      {
        PrevImage();
      }
      if (e.Key == Key.R)
      {
        ShowImage(images[imgIdx]);
      }
    }

    private void Image_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        mouseImgPos = new Vector2D(e.GetPosition(image));
        isMove = true;
      }
    }

    private void Window_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Released)
      {
        isMove = false;
      }
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
      mousePos = new Vector2D(e.GetPosition(this));
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      thread.Abort();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        var p = new Vector2D(e.GetPosition(this));
        if (!(imgMargin.X < p.X && 
              imgMargin.Y < p.Y && 
              imgMargin.X + imgSize.X > p.X && 
              imgMargin.Y + imgSize.Y > p.Y))
        {
          Application.Current.Shutdown();
        }
      }
      if (e.ChangedButton == MouseButton.XButton1)
      {
        PrevImage();
      }
      if (e.ChangedButton == MouseButton.XButton2)
      {
        NextImage();
      }
    }
  }
}
