using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFNha
{
    /// <summary>
    /// Interaction logic for QuanLyHopDong.xaml
    /// </summary>
    public partial class QuanLyHopDong : Window
    {
        DataClassesDataContext dc = new DataClassesDataContext();
        Table<HOPDONG> HOPDONGs;
        Table<NHA> NHAs;
        Table<KHACHTHUENHA> KHACHTHUENHAs;
        public QuanLyHopDong()
        {
            InitializeComponent();
            this.Loaded += QuanLyHopDong_Loaded;

            DataGrid.SelectionChanged += new SelectionChangedEventHandler(Data_Click);
        }

        private void Data_Click(object sender, SelectionChangedEventArgs e)
        {
        }

        private void QuanLyHopDong_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var query = from hd in dc.HOPDONGs
                        join nha in dc.NHAs on hd.MaNha equals nha.MaNha
                        join khach in dc.KHACHTHUENHAs on hd.MaKhach equals khach.MaKhach
                        select new
                        {
                            hd.SoHD,
                            TenChuNha = nha.TenChuNha,
                            TenKhachHang = khach.TenKhach,
                            hd.NgayBatDau,
                            hd.NgayKetThuc
                        };

            // Thực hiện truy vấn và chuyển đổi thành danh sách
            var result = query.ToList().Select(hd => new
            {
                hd.SoHD,
                hd.TenChuNha,
                hd.TenKhachHang,
                NgayBatDau = hd.NgayBatDau.ToString("dd/MM/yyyy"),
                NgayKetThuc = hd.NgayKetThuc.ToString("dd/MM/yyyy")
            }).ToList();

            DataGrid.ItemsSource = result;
        }

    }
}
