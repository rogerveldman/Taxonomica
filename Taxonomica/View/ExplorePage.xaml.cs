﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Taxonomica.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Taxonomica
{
    public sealed partial class ExplorePage : Page
    {
        public ExplorePage()
        {
            InitializeComponent();

            Task.Run(async () =>
            {
                await DispatcherUtil.Dispatch(async () =>
                {
                    var tsnList = new List<string> { "174371", "173420", "161061", "159785" };
                    var exploreItems = new ObservableCollection<ExploreItem>();

                    ExploreGrid.SetBinding(ListView.ItemsSourceProperty, new Binding { Source = exploreItems });

                    var tasks = tsnList.Select(async (tsn) =>
                    {
                        var newItem = new ExploreItem();
                        exploreItems.Add(newItem);

                        newItem.Loading = true;

                        var record = await RequestManager.RequestFullRecord(tsn);

                        newItem.Name = record.GetCommonName();
                        newItem.TSN = record.ScientificName.TSN;

                        var imageModelLow = await RequestManager.RequestWikispeciesImage(record.ScientificName.CombinedName, 50);
                        newItem.Image = new BitmapImage(new Uri(imageModelLow.GetThumbnail(), UriKind.Absolute));

                        var imageModel = await RequestManager.RequestWikispeciesImage(record.ScientificName.CombinedName, 400);
                        newItem.Image = new BitmapImage(new Uri(imageModel.GetThumbnail(), UriKind.Absolute));

                        newItem.Loading = false;
                    });

                    await Task.WhenAll(tasks);
                });
            });
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tsn = (((Grid)sender).DataContext as ExploreItem).TSN;
            Frame.Navigate(typeof(TaxonPage), new TaxonPageNavigationArgs { TSN = tsn });
        }
    }
}
