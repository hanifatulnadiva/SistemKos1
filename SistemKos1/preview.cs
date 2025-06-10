﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemKos1
{
    public partial class preview : Form
    {
        string connectionString = "Server=HANIFATUL-NADIV\\HANIFA; Database=SistemManagementKost;Trusted_Connection=True;";
        public preview(DataTable data)
        {
            InitializeComponent();
            dgvPreviewPenyewa.DataSource = data;
        }
        //event ketika form di muat
        private void PreviewData_Load(object sender, EventArgs e)
        {
            //optional: sesuaikan datagridview jika perlu
            dgvPreviewPenyewa.AutoResizeColumns(); //menyesuaikan ukuran kolom
        }

        private bool ValidateRow(DataRow row)
        {
            string NIM = row["NIK"].ToString();
            //validasi Nim (misalnya harus berjumlah 11 karakter
            if (NIM.Length != 11)
            {
                MessageBox.Show("nim harus terdiri dari 16 karakter.", "kesalahan validasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;

            }

            //jika perlu tambahkan validasi lain sesuai dengan kebutuhan misalnya pola tertentu untuk nim
            return true;
        }

        private void ImportDataToDatabase()
        {
            try
            {
                DataTable dt = (DataTable)dgvPreviewPenyewa.DataSource;

                foreach (DataRow row in dt.Rows)
                {
                    //validasi setiap baris sebelum di import
                    if (!ValidateRow(row))
                    {
                        //jika validasi gagal maka lanjutkan ke baris berikutnya 
                        continue; //lewati baris ini jika tidak valid

                    }

                    string query = "insert into Mahasiswa (NIK, nama, kontak, tanggal_masuk, tanggal_keluar) values (@NIK, @nama, @kontak, @tanggal_masuk, @tanggal_keluar)";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())

                        {
                            cmd.CommandText = query;

                            cmd.Parameters.AddWithValue("@NIK", row["NIM"]);
                            cmd.Parameters.AddWithValue("@nama", row["Nama"]);
                            cmd.Parameters.AddWithValue("@kontak", row["Email"]);
                            cmd.Parameters.AddWithValue("@tanggal_masuk", row["tanggal_masuk"]);
                            cmd.Parameters.AddWithValue("@tanggal_keluar", row["tanggal_keluar"]);

                            cmd.ExecuteNonQuery();

                        }
                    }
                }
                MessageBox.Show("data berhasil diimport ke database", "sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();//tutup preview setelah data diimport
            }
            catch (Exception ex)
            {
                MessageBox.Show("terjadi kesalahan saat mengimport data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPreviewPenyewa(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            //menanyakan kepada pengguna jika mereka ingin mengimpord data 
            DialogResult result = MessageBox.Show("Apakah anda ingin mengimport data ini ke database?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                //mengimport data dari datagrideview ke database
                ImportDataToDatabase();
            }
        }
    }
}
