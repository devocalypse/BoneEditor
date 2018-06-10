using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace BoneEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, KnownElements> KnownDescriptors = new Dictionary<string, KnownElements>();

        public MainWindow()
        {
            InitializeComponent();
            var descriptorFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BoneDescriptor.xml");

            if (File.Exists(descriptorFile))
                LoadNewDescriptor(descriptorFile, false);
        }

        private string LastFile { get; set; }
        private bool FileLoaded { get; set; }
        public List<Bone> Bones { get; set; } = new List<Bone>();

        private void DropFile(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            if (Path.GetExtension(files[0]) == ".txt")
                LoadBoneTextFile(files[0]);
            else if (Path.GetExtension(files[0]) == ".xml")
                LoadNewDescriptor(files[0], true);
        }

        private void LoadNewDescriptor(string file, bool reloadBones = false)
        {
            XDocument doc;

            try
            {
                doc = XDocument.Load(file);
            }
            catch (XmlException e)
            {
                MessageBox.Show(
                    "Failed to load descriptor XML.\n" + e.Message +
                    "\n\nFix the error and drag the xml over the main window or restart the program.", "XML Load Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var alldefs = doc.Root.Descendants("BoneDefinition");
            KnownDescriptors.Clear();
            foreach (var element in alldefs)
                KnownDescriptors.Add(element.Attribute("Code").Value,
                    new KnownElements
                    {
                        Group = element.Attribute("Group").Value,
                        Name = element.Attribute("Name").Value
                    });
            if (reloadBones)
                ReassignDescriptors();
        }

        private void ReassignDescriptors()
        {
            if (!KnownDescriptors.Any() || !Bones.Any()) return;
            foreach (var bone in Bones)
            {
                bone.Group = KnownDescriptors.ContainsKey(bone.Code)
                    ? string.IsNullOrEmpty(KnownDescriptors[bone.Code].Group)
                        ? "Unknown"
                        : KnownDescriptors[bone.Code].Group
                    : "Unknown";
                bone.Name = KnownDescriptors.ContainsKey(bone.Code)
                    ? string.IsNullOrEmpty(KnownDescriptors[bone.Code].Name)
                        ? "Unavailable"
                        : KnownDescriptors[bone.Code].Name
                    : "Unavailable";
            }
        }

        public void SaveBoneTextFile()
        {
            if (!FileLoaded) return;
            var sb = new StringBuilder();
            foreach (var bone in Bones)
                sb.AppendLine(bone.ToString());

            try
            {
                File.WriteAllText(LastFile, sb.ToString());
            }
            catch (IOException ex)
            {
                MessageBox.Show("Failed to update file.");
            }
        }

        public void LoadBoneTextFile(string s)
        {
            FileLoaded = false;
            Bones.Clear();
            var EnUsCultureInfo = CultureInfo.GetCultureInfo("en-US");
            foreach (var line in File.ReadLines(s))
            {
                var parts = line.Split(',');
                var b = new Bone
                {
                    Id = int.Parse(parts[0]),
                    Code = parts[1],
                    X = double.Parse(parts[3], EnUsCultureInfo),
                    Y = double.Parse(parts[4], EnUsCultureInfo),
                    Z = double.Parse(parts[5], EnUsCultureInfo),
                    B = double.Parse(parts[6], EnUsCultureInfo)
                };
                Bones.Add(b);
            }

            //Assign descriptors from xml
            ReassignDescriptors();

            //Order honoring descriptors
            Bones = Bones.OrderBy(b => b.Group).ThenBy(b => b.Name).ThenBy(b => b.Code).ToList();

            bonesview.ItemsSource = Bones;
            var view = (CollectionView) CollectionViewSource.GetDefaultView(bonesview.ItemsSource);
            using (view.DeferRefresh())
            {
                var groupDescription = new PropertyGroupDescription("Group");
                view.GroupDescriptions.Clear();
                view.GroupDescriptions.Add(groupDescription);
            }

            FileLoaded = true;
            LastFile = s;
        }

        private void FilterChanged(object sender, KeyEventArgs e)
        {
            var query = Filterbox.Text.Trim();
            filterModOnlyCb.IsChecked = false;

            var view = (CollectionView) CollectionViewSource.GetDefaultView(bonesview.ItemsSource);
            if (query.Length < 2) view.Filter = null;
            else
                view.Filter = item =>
                    item is Bone vitem &&
                    (Contains(vitem.Name, Filterbox.Text, StringComparison.CurrentCultureIgnoreCase) ||
                     Contains(vitem.Code, Filterbox.Text, StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool Contains(string source, string toCheck, StringComparison comp)
        {
            if (string.IsNullOrEmpty(toCheck) || string.IsNullOrEmpty(source))
                return false;

            return source.IndexOf(toCheck, comp) >= 0;
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SaveBoneTextFile();
        }

        private void ToggleModifyOnlyFilter(object sender, RoutedEventArgs e)
        {
            var s = sender as CheckBox;
            var view = (CollectionView) CollectionViewSource.GetDefaultView(bonesview.ItemsSource);

            if (s.IsChecked == true)
                view.Filter = item => item is Bone vitem && vitem.Enabled;
            else
                view.Filter = null;
        }

        private void ResetSliderValue(object sender, MouseButtonEventArgs e)
        {
            var s = sender as Slider;
            s.Value = 1;
        }


        public class KnownElements
        {
            public string Name { get; set; }
            public string Group { get; set; }
        }
    }
}