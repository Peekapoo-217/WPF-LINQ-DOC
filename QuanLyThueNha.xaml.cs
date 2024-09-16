using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    /// Interaction logic for QuanLyThueNha.xaml
    /// </summary>
    public partial class QuanLyThueNha : Window
    {
        DataClassesDataContext dc = new DataClassesDataContext();  //Đặt tên context như tên class .dbml
        Table<KHACHTHUENHA> KHACHTHUENHAs;
        public QuanLyThueNha()
        {
            InitializeComponent();
            this.Loaded += QuanLyThueNha_Loaded;

            btnThem.Click += new RoutedEventHandler(btnthem_click);
            btnSua.Click += new RoutedEventHandler(btnsua_Click);
            btnXoa.Click += new RoutedEventHandler(btnxoa_Click);
            btnLamMoi.Click += new RoutedEventHandler(btnLamMoi_Click);

            DataGrid.SelectionChanged += new SelectionChangedEventHandler(Data_Click);
        }

        private void Data_Click(object sender, SelectionChangedEventArgs e)
        {
            KHACHTHUENHA kh = DataGrid.SelectedItem as KHACHTHUENHA; //SelectedItem không có "s"
            if (kh != null)
            {
                txtMaKh.Text = kh.MaKhach.ToString();
                txtTenKh.Text = kh.TenKhach.ToString();
                dtpNgaySinh.Text = kh.NgaySinh.ToString();
                string gt = kh.GioiTinh.ToString();
                if (gt.Equals("True"))
                    rdNam.IsChecked = true;
                else
                    rdNu.IsChecked = true;
            }
        }

        public void loadKhachThueNha()
        {
            KHACHTHUENHAs = dc.GetTable<KHACHTHUENHA>();
            var query = from cv in KHACHTHUENHAs
                        select cv;

            DataGrid.ItemsSource = query;
        }

        private void QuanLyThueNha_Loaded(object sender, RoutedEventArgs e)
        {
            loadKhachThueNha();
        }

        private void btnthem_click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra tính hợp lệ của dữ liệu nhập vào trước khi thực hiện thêm
            if (string.IsNullOrEmpty(txtMaKh.Text.Trim()))
            {
                MessageBox.Show("Mã khách hàng không được để trống!");
                return;
            }

            // Kiểm tra xem mã khách hàng trùng lặp
            string maKhachHang = txtMaKh.Text.Trim();
            var khachHangTonTai = KHACHTHUENHAs.FirstOrDefault(khLambda => khLambda.MaKhach == maKhachHang);
            if (khachHangTonTai != null)
            {
                MessageBox.Show("Mã khách hàng không được trùng, vui lòng chọn mã khác!");
                return;
            }

            if (string.IsNullOrEmpty(txtTenKh.Text.Trim()))
            {
                MessageBox.Show("Tên khách hàng không được để trống!");
                return;
            }

            // Tạo một đối tượng KHACHTHUENHA mới và gán các giá trị từ các điều khiển nhập liệu
            KHACHTHUENHA kh = new KHACHTHUENHA();
            kh.MaKhach = txtMaKh.Text.Trim();
            kh.TenKhach = txtTenKh.Text.Trim();
            kh.GioiTinh = rdNam.IsChecked == true ? true : false;

            // Kiểm tra và gán ngày sinh từ DatePicker
            if (dtpNgaySinh.SelectedDate.HasValue)
            {
                kh.NgaySinh = dtpNgaySinh.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ngày sinh!");
                return;
            }

            // Thêm khách thuê nhà vào bảng KHACHTHUENHA
            KHACHTHUENHAs.InsertOnSubmit(kh);

            try
            {
                // Cập nhật dữ liệu xuống Database
                dc.SubmitChanges();
                MessageBox.Show("Thêm khách thuê nhà thành công!");
                loadKhachThueNha();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnsua_Click(object sender, RoutedEventArgs e)
        {
            //Lấy thông tin khách hàng từ datagrid
            KHACHTHUENHA choice = DataGrid.SelectedItem as KHACHTHUENHA;
            if(choice == null)
            {
                MessageBox.Show("Không thể lấy thông tin khách hàng");
                return;
            }

            //Cập nhật các trường nhập liệu
            choice.MaKhach = txtMaKh.Text.Trim();
            choice.TenKhach = txtTenKh.Text.Trim();
            choice.GioiTinh = rdNam.IsChecked == true ? true : false;

            //Thực hiện sửa
            try
            {
                //Cập nhật dô cơ sở dữ liệu
                dc.SubmitChanges();
                MessageBox.Show("Đã sửa thông tin khách hàng");
                loadKhachThueNha();
            }catch(Exception exp)
            {
                MessageBox.Show("Lỗi khi cập nhật thông tin khách hàng!", exp.Message);
            }
        }

        private void btnxoa_Click(object sender, RoutedEventArgs e)
        {
            KHACHTHUENHA kh = DataGrid.SelectedItem as KHACHTHUENHA;
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    KHACHTHUENHAs.DeleteOnSubmit(kh);
                    dc.SubmitChanges();
                    MessageBox.Show("Xóa thành công");
                    loadKhachThueNha();
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Lỗi khi xóa: " + exp.Message);
                }
            }
        }

        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            txtMaKh.Text = "";
            txtTenKh.Text = "";

            rdNam.IsChecked = false;
            rdNu.IsChecked = false;

            dtpNgaySinh.SelectedDate = null;
        }


        private void btnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            //Trim cắt khoảng trốngs
            string tuKhoa = txtTimKiem.Text.Trim();

            // Kiểm tra xem có mã khách hàng nào chứa từ khóa tìm kiếm không
            var ketQuaTimKiem = KHACHTHUENHAs.FirstOrDefault(kh => kh.MaKhach == tuKhoa);

            if (ketQuaTimKiem != null)
            {
                // Hiển thị thông tin khách hàng tìm thấy lên các control
                txtMaKh.Text = ketQuaTimKiem.MaKhach;
                txtTenKh.Text = ketQuaTimKiem.TenKhach;
                dtpNgaySinh.SelectedDate = ketQuaTimKiem.NgaySinh;
                if (ketQuaTimKiem.GioiTinh)
                {
                    rdNam.IsChecked = true;
                    rdNu.IsChecked = false;
                }
                else
                {
                    rdNam.IsChecked = false;
                    rdNu.IsChecked = true;
                }
            }
            else
            {
                // Hiển thị thông báo nếu không tìm thấy khách hàng
                MessageBox.Show("Không có người này");
                return;
            }
        }
    }
}
